namespace E_CommerceAPI.ENTITES.DTOs.PaymentDTO
{
    public class CheckoutResponse
    {
        public string? Url { get; set; }
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
