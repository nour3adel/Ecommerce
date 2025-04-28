namespace E_CommerceAPI.ENTITES.DTOs.AutherizationDTOs
{
    public class ManageUserRolesResult
    {
        public string UserId { get; set; } = null!;
        public List<UserRoles> userRoles { get; set; }
    }
}
