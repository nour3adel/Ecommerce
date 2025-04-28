namespace E_CommerceAPI.ENTITES.DTOs.AccountDTO
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
