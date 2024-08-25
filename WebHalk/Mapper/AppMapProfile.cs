using AutoMapper;
using WebHalk.Data.Entities;
using WebHalk.Models.Categories;
using WebHalk.Models.Products;

namespace WebHalk.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            CreateMap<CategoryEntity, CategoryEditViewModel>();

            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(x => x.Images, opt => opt.MapFrom(x => x.ProductImages.Select(p => p.Image).ToArray()));

            CreateMap<ProductEntity, ProductEditViewModel>()
                .ForMember(dest => dest.NewPhotos, opt => opt.Ignore()) // Якщо NewPhotos не використовується під час редагування
                .ForMember(dest => dest.CategoryList, opt => opt.Ignore()); // Якщо CategoryList не використовується під час редагування

            CreateMap<ProductCreateViewModel, ProductEntity>();
            CreateMap<ProductEditViewModel, ProductEntity>();
        }
    }
}
