using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.DTOs.StripeConfig;
using E_CommerceAPI.ENTITES.Models;
using E_CommerceAPI.Infrastructure.UOW;
using E_CommerceAPI.SERVICES.Bases;
using E_CommerceAPI.SERVICES.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Net;

namespace E_CommerceAPI.SERVICES.Implementation
{
    public class PaymentService : ResponseHandler, IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly StripeConfig _stripeConfig;
        private readonly IOrderService _orderService;
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, StripeConfig stripeConfig, IOrderService orderService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _stripeConfig = stripeConfig;
            _orderService = orderService;

        }

        private async Task<Payment> CreatePayment(PaymentDto dto, decimal amount, ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "PaymentDto cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dto.CurrencyCode))
            {
                throw new ArgumentException("Currency code is required.", nameof(dto.CurrencyCode));
            }

            var payment = new Payment
            {
                Method = dto.Method ?? "card",
                Currency = dto.CurrencyCode.ToUpper(), // Changed to uppercase for consistency
                Description = dto.Description ?? "No description provided",
                Amount = amount,
                CustomerId = user.Id,
                Customer = user,
                Date = DateTime.UtcNow // Added creation timestamp
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.Save(); // Changed to SaveAsync for consistency

            return payment;
        }

        public async Task<Response<CheckoutResponse>> CreateCheckoutSession(PaymentDto dto, ApplicationUser user)
        {
            // Validate inputs at the start
            if (user == null)
            {
                return BadRequest<CheckoutResponse>("User is required.");
            }

            if (dto == null)
            {
                return BadRequest<CheckoutResponse>("Payment details are required.");
            }

            // Step 1: Validate the cart
            var cart = await _unitOfWork.Carts.GetTableNoTracking()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == user.Id && !c.IsClosed);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest<CheckoutResponse>("Your cart is empty. Please add items before proceeding.");
            }

            // Check stock availability
            var outOfStockItem = cart.CartItems.FirstOrDefault(item => item.Quantity > item.Product.StockAmount);
            if (outOfStockItem != null)
            {
                await _unitOfWork.Carts.DeleteAsync(cart);
                await _unitOfWork.Save();
                return BadRequest<CheckoutResponse>($"Product '{outOfStockItem.Product.Name}' has only {outOfStockItem.Product.StockAmount} items in stock.");
            }

            // Step 2: Validate shipping details
            if (dto.ShippingDetails?.Address == null)
            {
                return BadRequest<CheckoutResponse>("Shipping address is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.ShippingDetails.Country))
            {
                return BadRequest<CheckoutResponse>("Shipping country is required.");
            }

            // Step 3: Validate currency code
            if (!IsValidCurrency(dto.CurrencyCode))
            {
                return BadRequest<CheckoutResponse>("Invalid currency code. Please provide a valid currency.");
            }

            // Step 4: Calculate pricing
            decimal subtotal = cart.CartItems.Sum(item => item.Quantity * (decimal)item.Product.Price);
            decimal shippingCost = (decimal)dto.ShippingDetails.Cost;
            decimal totalAmount = subtotal + shippingCost;

            // Step 5: Create payment record
            var payment = await CreatePayment(new PaymentDto
            {
                CurrencyCode = dto.CurrencyCode,
                Description = "Payment for items in cart",
                Method = dto.Method,
                ShippingDetails = dto.ShippingDetails
            }, totalAmount, user);

            if (payment == null)
            {
                return BadRequest<CheckoutResponse>("Failed to create payment record.");
            }

            // Create Stripe customer
            Customer customer;
            try
            {
                var customerService = new CustomerService();
                customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}",
                    Address = new AddressOptions
                    {
                        Line1 = dto.ShippingDetails.Address,
                        City = dto.ShippingDetails.City,
                        PostalCode = dto.ShippingDetails.ZipCode,
                        State = dto.ShippingDetails.State,
                        Country = dto.ShippingDetails.Country
                    }
                });
            }
            catch (StripeException ex)
            {
                return BadRequest<CheckoutResponse>($"Failed to create customer: {ex.Message}");
            }

            // Step 6: Prepare Stripe checkout session
            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = _stripeConfig.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = _stripeConfig.CancelUrl,
                Customer = customer.Id,
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { dto.ShippingDetails.Country.ToUpper() }
                },
                LineItems = cart.CartItems.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = payment.Currency.ToLower(),
                        UnitAmount = (long)(item.Product.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description
                        }
                    }
                }).ToList(),
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
            {
                { "customer_id", user.Id },
                { "customer_name", $"{user.FirstName} {user.LastName}" },
                { "address", dto.ShippingDetails.Address },
                { "city", dto.ShippingDetails.City },
                { "state", dto.ShippingDetails.State },
                { "postal_code", dto.ShippingDetails.ZipCode },
                { "country", dto.ShippingDetails.Country }
            }
                }
            };

            // Step 7: Create order and commit transaction
            using var transaction = await _unitOfWork.Orders.BeginTransactionAsync();
            try
            {
                var order = new ENTITES.Models.Order
                {
                    CustomerId = user.Id,
                    Status = "open",
                    ShippingAddress = dto.ShippingDetails.Address,
                    ShippingMethod = dto.ShippingDetails.Method,
                    ShippingDate = DateTime.UtcNow,
                    DeliverDate = DateTime.UtcNow.AddDays(7),
                    ShippingCost = (double)shippingCost,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = cart.CartItems.Select(item => new OrderItem
                    {
                        Discount = (decimal)item.Product.DiscountPercentage,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = (decimal)item.Product.Price
                    }).ToList()
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.Save();

                // Add order ID to session metadata after we have it
                sessionOptions.PaymentIntentData.Metadata["order_id"] = order.Id.ToString();

                // Create Stripe session
                Session session;
                try
                {
                    var sessionService = new SessionService();
                    session = await sessionService.CreateAsync(sessionOptions);
                }
                catch (StripeException ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest<CheckoutResponse>($"Failed to create checkout session: {ex.Message}");
                }

                // Update product stock
                foreach (var item in cart.CartItems)
                {
                    item.Product.StockAmount -= item.Quantity;
                    await _unitOfWork.Products.UpdateAsync(item.Product);
                }

                // Close cart and mark order as complete
                cart.IsClosed = true;
                order.Status = "completed"; // Changed from "closed" to "completed" for clarity

                await _unitOfWork.Carts.UpdateAsync(cart);
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.Save();

                await transaction.CommitAsync();

                CheckoutResponse checkoutResponse = new CheckoutResponse
                {
                    Url = session.Url,
                    SuccessUrl = session.SuccessUrl,
                    CancelUrl = session.CancelUrl
                };
                return new Response<CheckoutResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Succeeded = true,
                    Data = checkoutResponse,
                    Message = "Checkout session created successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Response<CheckoutResponse>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Succeeded = false,
                    Message = $"An error occurred while creating the checkout session: {ex.Message}"
                };
            }
        }


        // Reusable helper for error responses



        public async Task<Response<string>> DeletePayment(int id)
        {
            var pay = await _unitOfWork.Payments.GetByIdAsync(id);
            if (pay != null)
            {
                var dto = _mapper.Map<PaymentDto>(pay);
                await _unitOfWork.Payments.DeleteAsync(pay);
                await _unitOfWork.Save();
                return Deleted<string>("Payment deleted successfully.");
            }
            return NotFound<string>("Payment not found.");
        }

        public async Task<Response<IEnumerable<PaymentDto>>> GetAllPayments()
        {
            var pay = await _unitOfWork.Payments.GetTableNoTracking().ToListAsync();
            if (pay != null && pay.Count > 0)
            {
                var dto = _mapper.Map<IEnumerable<PaymentDto>>(pay);
                return Success(dto);
            }
            return NotFound<IEnumerable<PaymentDto>>("User did not make any payments.");

        }

        public async Task<Response<PaymentDto>> GetPayment(int id)
        {
            var pay = await _unitOfWork.Payments.GetByIdAsync(id);
            if (pay != null)
            {
                var dto = _mapper.Map<PaymentDto>(pay);
                return Success(dto);
            }
            return NotFound<PaymentDto>("Payment not found.");
        }

        // Helpers

        //---- Currency Validation --------

        private bool IsValidCurrency(string currency)
        {
            return SupportedCurrencies.Contains(currency.ToLower());
        }

        private readonly HashSet<string> SupportedCurrencies = new HashSet<string>
        {
            "usd", "aed", "afn", "all", "amd", "ang", "aoa", "ars", "aud", "awg", "azn", "bam",
            "bbd", "bdt", "bgn", "bhd", "bif", "bmd", "bnd", "bob", "brl", "bsd", "bwp", "byn",
            "bzd", "cad", "cdf", "chf", "clp", "cny", "cop", "crc", "cve", "czk", "djf", "dkk",
            "dop", "dzd", "egp", "etb", "eur", "fjd", "fkp", "gbp", "gel", "gip", "gmd", "gnf",
            "gtq", "gyd", "hkd", "hnl", "hrk", "htg", "huf", "idr", "ils", "inr", "isk", "jmd",
            "jod", "jpy", "kes", "kgs", "khr", "kmf", "krw", "kwd", "kyd", "kzt", "lak", "lbp",
            "lkr", "lrd", "lsl", "mad", "mdl", "mga", "mkd", "mmk", "mnt", "mop", "mur", "mvr",
            "mwk", "mxn", "myr", "mzn", "nad", "ngn", "nio", "nok", "npr", "nzd", "omr", "pab",
            "pen", "pgk", "php", "pkr", "pln", "pyg", "qar", "ron", "rsd", "rub", "rwf", "sar",
            "sbd", "scr", "sek", "sgd", "shp", "sle", "sos", "srd", "std", "szl", "thb", "tjs",
            "tnd", "top", "try", "ttd", "twd", "tzs", "uah", "ugx", "uyu", "uzs", "vnd", "vuv",
            "wst", "xaf", "xcd", "xof", "xpf", "yer", "zar", "zmw", "usdc", "btn", "ghs", "eek",
            "lvl", "svc", "vef", "ltl", "sll", "mro"
        };
    }
}
