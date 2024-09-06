using AutoMapper;
using System.Globalization;
using WebHalk.Data.Entities;
using WebHalk.Data.Entities.Identity;
using WebHalk.Models;
using WebHalk.Models.Account;
using WebHalk.Models.Categories;
using WebHalk.Models.Products;
using WebHalk.Areas.Admin.Models.Category;
using WebHalk.Areas.Admin.Models.Products;

namespace WebHalk.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            // Мапінг для категорій (існуючий)
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            CreateMap<CategoryEntity, CategoryEditViewModel>();

            // Мапінг для категорій (додавання для адміністративної частини)
            CreateMap<CategoryEntity, CategoryAdminItemViewModel>();
            CreateMap<CategoryEntity, CategoryAdminCreateViewModel>();
            CreateMap<CategoryEntity, CategoryAdminEditViewModel>();

            // Мапінг для продуктів (існуючий)
            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(x => x.Images, opt => opt.MapFrom(x => x.ProductImages.Select(p => p.Image).ToArray()));

            CreateMap<ProductEntity, ProductEditViewModel>()
                .ForMember(x => x.Images,
                    opt => opt.MapFrom(src => src.ProductImages.Select(pi => new ProductImageViewModel
                    {
                        Id = pi.Id,
                        Name = "/images/" + pi.Image,
                        Priority = pi.Priotity
                    }).ToList()))
                .ForMember(x => x.Price, opt => opt.MapFrom(x => x.Price.ToString(new CultureInfo("uk-UA"))));

            CreateMap<ProductEditViewModel, ProductEntity>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Price, opt => opt.MapFrom(x => Decimal.Parse(x.Price, new CultureInfo("uk-UA"))));

            // Мапінг для користувачів (існуючий)
            CreateMap<UserEntity, ProfileViewModel>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"));

            // Додавання мапінгів для адміністративної частини продуктів
            CreateMap<ProductEntity, ProductAdminItemViewModel>();

            CreateMap<ProductEntity, ProductAdminCreateViewModel>()
                .ForMember(x => x.CategoryList, opt => opt.Ignore())
                .ForMember(x => x.Photos, opt => opt.Ignore());

            CreateMap<ProductEntity, ProductAdminEditViewModel>()
                .ForMember(x => x.CategoryList, opt => opt.Ignore())
                .ForMember(x => x.Images, opt => opt.MapFrom(src => src.ProductImages.Select(pi => new ProductAdminImageViewModel
                {
                    Id = pi.Id,
                    Name = "/images/" + pi.Image,
                    Priority = pi.Priotity
                }).ToList()))
                .ForMember(x => x.NewImages, opt => opt.Ignore())
                .ForMember(x => x.DeletedPhotoIds, opt => opt.Ignore());
        }
    }
}
