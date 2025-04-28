namespace Ecommerce.Frontend.Models.Payment
{
    public class CheckoutResponse
    {
        public string? Url { get; set; }
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
