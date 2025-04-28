namespace E_CommerceAPI.ENTITES.DTOs.CategoryDTO
{
    public class CategoryDetailedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int TotalStock { get; set; }

    }
}
