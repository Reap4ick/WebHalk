using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebHalk.Data;
using WebHalk.Models.Products;
using AutoMapper.QueryableExtensions;

public class ProductsController : Controller
{
    private readonly HulkDbContext _context;
    private readonly IMapper _mapper;

    public ProductsController(HulkDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IActionResult Index(ProductSearchViewModel search)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(search.Name))
            query = query.Where(x => x.Name.ToLower().Contains(search.Name.ToLower()));

        int count = query.Count();
        int page = search.Page ?? 1;
        int pageSize = search.PageSize;

        query = query.OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var list = query
              .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
              .ToList();

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        // Get categories for the search dropdown
        var categories = _context.Categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        search.Categories = categories;

        var model = new ProductHomeViewModel
        {
            Search = search,
            Products = list,
            Count = count,
            Pagination = new PaginationViewModel
            {
                PageSize = pageSize,
                TotalItems = count,
                CurrentPage = page,
            }
        };

        // Use ILogger for logging
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ProductsController>();
        logger.LogInformation("RunTime ProductsController Index: " + elapsedTime);

        return View(model);
    }
}
