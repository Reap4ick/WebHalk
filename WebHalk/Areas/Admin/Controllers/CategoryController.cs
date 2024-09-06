using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Areas.Admin.Models.Category;
using Microsoft.EntityFrameworkCore;
using WebHalk.Constants;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;

namespace WebHalk.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .ProjectTo<CategoryAdminItemViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryAdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Image != null)
            {
                // Генеруємо унікальне ім'я файлу
                string ext = Path.GetExtension(model.Image.FileName);
                string fileName = Guid.NewGuid().ToString() + ext;

                // Задаємо шлях до папки images
                var path = Path.Combine("C:\\Users\\User\\Desktop\\Neww\\WebHalk-master\\WebHalk\\images", fileName);

                // Зберігаємо файл
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                // Створюємо нову категорію з цим зображенням
                CategoryEntity entity = new CategoryEntity
                {
                    Image = fileName,
                    Name = model.Name
                };

                _context.Categories.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Categories
                .Where(c => c.Id == id)
                .ProjectTo<CategoryAdminEditViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Item with id={id} doesn’t exist");

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryAdminEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = await _context.Categories.FindAsync(model.Id)
                ?? throw new InvalidOperationException("Category not found");

            item.Name = model.Name;
            var imagesPath = "C:\\Users\\User\\Desktop\\Neww\\WebHalk-master\\WebHalk\\images";

            if (model.NewImage != null)
            {
                // Видаляємо старе зображення, якщо воно існує
                var currentImg = Path.Combine(imagesPath, item.Image);
                if (System.IO.File.Exists(currentImg))
                    System.IO.File.Delete(currentImg);

                // Завантажуємо нове зображення
                var newImg = Guid.NewGuid().ToString() + Path.GetExtension(model.NewImage.FileName);
                var newPath = Path.Combine(imagesPath, newImg);
                using (var stream = new FileStream(newPath, FileMode.Create))
                    await model.NewImage.CopyToAsync(stream);

                item.Image = newImg;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.Categories.FindAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                var path = Path.Combine("C:\\Users\\User\\Desktop\\Neww\\WebHalk-master\\WebHalk\\images", item.Image);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                _context.Categories.Remove(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Логування помилки або інші дії
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
