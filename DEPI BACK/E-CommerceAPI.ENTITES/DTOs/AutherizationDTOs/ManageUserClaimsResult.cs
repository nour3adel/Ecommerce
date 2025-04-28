namespace E_CommerceAPI.ENTITES.DTOs.AutherizationDTOs
{
    public class ManageUserClaimsResult
    {
        public string UserId { get; set; } = null!;
        public List<UserClaims>? userClaims { get; set; }
    }
}
