namespace E_CommerceAPI.ENTITES.DTOs.BrandDTO
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
