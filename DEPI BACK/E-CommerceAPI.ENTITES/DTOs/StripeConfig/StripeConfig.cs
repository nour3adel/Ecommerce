namespace E_CommerceAPI.ENTITES.DTOs.StripeConfig
{
    public class StripeConfig
    {
        public string PublishableKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string SuccessUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;

    }
}
