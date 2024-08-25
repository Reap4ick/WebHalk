using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebHalk.Models.Products
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Назва")]
        [Required(ErrorMessage = "Вкажіть назву продукту")]
        [StringLength(500, ErrorMessage = "Назва продукту не повинна перевищувати 500 символів")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ціна")]
        [Required(ErrorMessage = "Вкажіть ціну продукту")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        public decimal Price { get; set; }

        [Display(Name = "Категорія")]
        [Required(ErrorMessage = "Оберіть категорію")]
        public int CategoryId { get; set; }

        [Display(Name = "Нові фото")]
        public List<IFormFile>? NewPhotos { get; set; }

        // Список усіх категорій
        public SelectList? CategoryList { get; set; }

    }
}
