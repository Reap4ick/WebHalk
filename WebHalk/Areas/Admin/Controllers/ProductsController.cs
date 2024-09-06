using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Areas.Admin.Models.Products;
using Microsoft.AspNetCore.Authorization;
using WebHalk.Constants;
using WebHalk.Models.Products;

namespace WebHalk.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductsController : Controller
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .ProjectTo<ProductAdminItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            if (products == null)
                throw new Exception("Failed to get products");

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            var viewModel = new ProductAdminCreateViewModel
            {
                CategoryList = new SelectList(categories, "Value", "Text")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductAdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = new ProductEntity
            {
                Name = model.Name,
                Price = model.Price,
                CategoryId = model.CategoryId
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    var productImage = new ProductImageEntity
                    {
                        Image = fileName,
                        Product = product
                    };

                    _context.ProductImages.Add(productImage);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _context.Products
                .ProjectTo<ProductAdminEditViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id)
                ?? throw new Exception("An error occurred while receiving the product");

            var categories = _context.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            model.CategoryList = new SelectList(categories, "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductAdminEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var editProduct = _context.Products
                    .ProjectTo<ProductAdminEditViewModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefault(x => x.Id == model.Id)
                    ?? throw new Exception("An error occurred while receiving the product");

                var categories = _context.Categories
                    .Select(x => new { Value = x.Id, Text = x.Name })
                    .ToList();

                model.CategoryList = new SelectList(categories, "Value", "Text");
                model.Images = editProduct.Images;

                return View(model);
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                ?? throw new Exception("No product was found");

            _mapper.Map(model, product);

            if (model.NewImages != null)
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        string ext = Path.GetExtension(img.FileName);
                        string fName = Guid.NewGuid().ToString() + ext;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fName);

                        using (var fs = new FileStream(path, FileMode.Create))
                            await img.CopyToAsync(fs);

                        var imgEntity = new ProductImageEntity
                        {
                            Image = fName,
                            Product = product
                        };
                        _context.ProductImages.Add(imgEntity);
                    }
                }
            }

            if (model.DeletedPhotoIds != null)
            {
                var photos = _context.ProductImages
                    .Where(pi => model.DeletedPhotoIds.Contains(pi.Id))
                    .ToList();

                _context.ProductImages.RemoveRange(photos);

                foreach (var photo in photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", photo.Image);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
