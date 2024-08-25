using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Models.Products;

namespace WebHalk.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HulkDbContext _hulkDbContext;
        private readonly IMapper _mapper;

        public ProductsController(HulkDbContext hulkDbContext, IMapper mapper)
        {
            _hulkDbContext = hulkDbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var list = _hulkDbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _hulkDbContext.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            var viewModel = new ProductCreateViewModel
            {
                CategoryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Створюємо новий продукт
            var product = new ProductEntity
            {
                Name = model.Name,
                Price = model.Price,
                CategoryId = model.CategoryId,
            };

            // Додаємо продукт до бази даних
            _hulkDbContext.Products.Add(product);
            await _hulkDbContext.SaveChangesAsync();

            // Додаємо фотографії
            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    string extension = Path.GetExtension(photo.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    var photoEntity = new ProductImageEntity
                    {
                        Image = fileName,
                        ProductId = product.Id,
                    };

                    _hulkDbContext.ProductImages.Add(photoEntity);
                }

                await _hulkDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _hulkDbContext.Products
                .Where(p => p.Id == id)
                .ProjectTo<ProductEditViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            // Отримуємо список категорій
            product.CategoryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _hulkDbContext.Categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    _hulkDbContext.Categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var product = await _hulkDbContext.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }

            // Оновлюємо дані продукту
            product.Name = model.Name;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            // Видаляємо всі старі фотографії продукту
            foreach (var image in product.ProductImages)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "images", image.Image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                _hulkDbContext.ProductImages.Remove(image);
            }

            // Додаємо нові фотографії
            if (model.NewPhotos != null && model.NewPhotos.Any())
            {
                foreach (var photo in model.NewPhotos)
                {
                    string extension = Path.GetExtension(photo.FileName);
                    string fileName = Guid.NewGuid().ToString() + extension;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    var photoEntity = new ProductImageEntity
                    {
                        Image = fileName,
                        ProductId = product.Id,
                    };

                    _hulkDbContext.ProductImages.Add(photoEntity);
                }
            }

            await _hulkDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _hulkDbContext.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            // Видаляємо всі фотографії продукту
            foreach (var image in product.ProductImages)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "images", image.Image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);

                _hulkDbContext.ProductImages.Remove(image);
            }

            _hulkDbContext.Products.Remove(product);
            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
