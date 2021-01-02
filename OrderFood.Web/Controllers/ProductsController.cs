using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain;
using OrderFood.Infrastructure.Data;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly OrderFoodContext _context;

        public ProductsController(OrderFoodContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            var viewModel = new ProductListViewModel { Products = products };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Show(long id)
        {
            var product = await _context.Products.FindAsync(id);
            var viewModel = new ProductModel()
            {
                Id = product.Id,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Description = product.Subtitle,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Title = model.Title,
                    Subtitle = model.Subtitle,
                    Description = model.Description,
                    Price = model.Price ?? 0,
                    ImageUrl = model.ImageUrl,
                };

                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                SetFlash(FlashMessageType.Success, $"Product \"{model.Title}\" was successfully created.");

                return RedirectToAction("Products", "Dashboard");
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductModel()
            {
                Id = product.Id,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                SelectedCategories = product.ProductCategories.Select(pc => pc.Category).Select(c => new CategoryModel
                {
                    Id = c.Id,
                    Title = c.Title
                }).ToList(),
            };

            ViewBag.Categories = await _context.Categories.Select(c => new CategoryModel
            {
                Id = c.Id,
                Title = c.Title
            }).ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductModel model, long[] categoryIds)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.Include(p => p.ProductCategories).FirstOrDefaultAsync(p => p.Id == model.Id);

                if (product == null)
                {
                    return NotFound();
                }

                product.Title = model.Title;
                product.Subtitle = model.Subtitle;
                product.Description = model.Description;
                product.Price = model.Price ?? 0;
                product.ImageUrl = model.ImageUrl;

                product.ProductCategories = categoryIds.Select(categoryId => new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                }).ToList();

                _context.Entry(product).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                SetFlash(FlashMessageType.Success, $"Product \"{model.Title}\" was successfully updated.");

                return RedirectToAction("Products", "Dashboard");
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            ViewBag.Categories = await _context.Categories.Select(c => new CategoryModel
            {
                Id = c.Id,
                Title = c.Title
            }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductModel()
            {
                Id = product.Id,
                Title = product.Title,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductModel model)
        {
            var product = await _context.Products.FindAsync(model.Id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            SetFlash(FlashMessageType.Success, $"Product \"{product.Title}\" was successfully deleted.");

            return RedirectToAction("Products", "Dashboard");
        }
    }
}
