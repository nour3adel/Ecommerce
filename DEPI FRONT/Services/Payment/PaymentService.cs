using Ecommerce.Frontend.Models.Common;
using Ecommerce.Frontend.Models.Payment;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce.Frontend.Services.Payment
{
    public class PaymentService : ResponseHandler, IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentService> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(
            HttpClient httpClient,
            ILogger<PaymentService> logger,
            IOptions<ApiSettings> apiSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apiSettings.Value;
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);
            _httpContextAccessor = httpContextAccessor;
            SetAuthorizationHeader();
        }

        private void SetAuthorizationHeader()
        {
            // Retrieve the access token from the session
            var accessToken = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
            Console.WriteLine($"Access Token: {accessToken}");

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                _logger.LogWarning("Access token not found in session. Authorization header not set.");
            }
        }

        private async Task<T> HandleApiResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<T>(json, options);
            }

            _logger.LogError($"API returned status code: {response.StatusCode}");
            return default(T);
        }

        public async Task<Response<List<PaymentDto>>> GetAllPayments()
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.PaymentEndpoint}/AllPayments";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<List<PaymentDto>>>(response)
                   ?? BadRequest<List<PaymentDto>>("Failed to fetch all payment Items.");
        }

        public async Task<Response<CheckoutResponse>> AddPayment(PaymentDto paymentDto)
        {
            try
            {
                SetAuthorizationHeader();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(paymentDto),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.PaymentEndpoint}/Checkout";
                var response = await _httpClient.PostAsync(endpoint, jsonContent);

                return await HandleApiResponseAsync<Response<CheckoutResponse>>(response)
                    ?? BadRequest<CheckoutResponse>("Failed to add payment.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing payment.");
                return new Response<CheckoutResponse>
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred while communicating with the server.",
                    Errors = new List<string> { "Failed to reach the server. Please try again later." }
                };
            }
        }

        public async Task<Response<PaymentDto>> GetPayment(int id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.PaymentEndpoint}/Payment/{id}";
            var response = await _httpClient.GetAsync(endpoint);

            return await HandleApiResponseAsync<Response<PaymentDto>>(response)
                   ?? BadRequest<PaymentDto>($"Failed to fetch payment with ID: {id}.");
        }

        public async Task<Response<string>> DeletePayment()
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> DeletePayment(int Id)
        {
            var endpoint = $"{ApiConstants.BaseUrl}{ApiConstants.PaymentEndpoint}/DeletePayment/{Id}";
            var response = await _httpClient.DeleteAsync(endpoint);

            return await HandleApiResponseAsync<Response<string>>(response)
                   ?? BadRequest<string>($"Failed to delete payment Item with ID: {Id}.");
        }
    }
}
