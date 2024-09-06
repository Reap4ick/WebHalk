using System.ComponentModel.DataAnnotations;

namespace WebHalk.Areas.Admin.Models.Category
{
    public class CategoryAdminEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }

        [Display(Name = "Оберіть фото на ПК")]
        public IFormFile? NewImage { get; set; }
    }
}