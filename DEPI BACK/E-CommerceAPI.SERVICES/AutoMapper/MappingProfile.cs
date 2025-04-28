using AutoMapper;
using E_CommerceAPI.ENTITES.DTOs.BrandDTO;
using E_CommerceAPI.ENTITES.DTOs.CartDTO;
using E_CommerceAPI.ENTITES.DTOs.CategoryDTO;
using E_CommerceAPI.ENTITES.DTOs.OrderDTO;
using E_CommerceAPI.ENTITES.DTOs.PaymentDTO;
using E_CommerceAPI.ENTITES.DTOs.ProductDTO;
using E_CommerceAPI.ENTITES.DTOs.ReviewDTO;
using E_CommerceAPI.ENTITES.DTOs.UserDTO;
using E_CommerceAPI.ENTITES.DTOs.WishlistDTO;
using E_CommerceAPI.ENTITES.Models;

namespace E_CommerceAPI.SERVICES.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // User-related mappings
            CreateMap<RegisterDto, ApplicationUser>().ReverseMap();
            CreateMap<UserDto, ApplicationUser>().ReverseMap();

            // Product-related mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name ?? "N/A"))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name ?? "N/A"))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<AddProductDto, Product>()
                  .ForMember(dest => dest.Images, opt => opt.Ignore())  // Manually handle Images
                  .ForMember(dest => dest.Colors, opt => opt.Ignore()); // Manually handle Colors

            // ProductImage-related mappings
            CreateMap<ProductImage, ImagesDto>()
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.ImageUrl))
                .ReverseMap();

            // ProductColors-related mappings
            CreateMap<ProductColor, ColorsDto>()
                .ForMember(dest => dest.color, opt => opt.MapFrom(src => src.ColorName))
                .ReverseMap();

            // Product-related mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name ?? "N/A"))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name ?? "N/A"))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap();

            CreateMap<Product, AddProductDto>().ReverseMap();

            // Order-related mappings
            CreateMap<Order, OrderDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name ?? "N/A"))
                .ReverseMap();

            // Category-related mappings
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, AddCategoryDto>().ReverseMap();

            // Brand-related mappings
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<Brand, AddBrandDto>().ReverseMap();

            // Cart-related mappings
            CreateMap<CartDto, Cart>()
                .ReverseMap()
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.IsClosed ? "closed" : "opened"));

            CreateMap<CartItem, CartItemsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name ?? "N/A"))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ReverseMap();

            // Wishlist-related mappings
            CreateMap<Wishlist, WishlistDto>().ReverseMap();

            CreateMap<WishlistItem, WishlistItemsDto>()
                .ForMember(dest => dest.WishlistName, opt => opt.MapFrom(src => src.Wishlist.Name ?? "N/A"))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name ?? "N/A"))
                .ReverseMap();

            // Review-related mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name ?? "N/A"))
                .ReverseMap();

            // Payment-related mappings
            CreateMap<Payment, PaymentDto>().ReverseMap();
        }
    }
}
