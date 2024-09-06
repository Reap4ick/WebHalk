using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebHalk.Areas.Admin.Models.Category
{
    public class CategoryAdminCreateViewModel
    {
        [Display(Name = "Назва")]
        [Required(ErrorMessage = "Вкажіть назву категорії")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Оберіть фото на ПК")]
        [Required(ErrorMessage = "Оберіть фото")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
    }
}
