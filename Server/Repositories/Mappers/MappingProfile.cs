// MappingProfile.cs
using AutoMapper;
using gamershop.Shared.DTOs;
using gamershop.Shared.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new ProductCategory { CategoryId = src.CategoryId })); // Map CategoryId to ProductCategory
        CreateMap<ProductDTO, Product>();
    }
}
