using System.Security.Claims;

namespace E_CommerceAPI.ENTITES.Helpers
{
    public static class ClaimsStore
    {
        public static List<Claim> claims = new()
        {
            new Claim("Create Product","false"),
            new Claim("Edit Product","false"),
            new Claim("Delete Product","false"),
        };

    }
}
