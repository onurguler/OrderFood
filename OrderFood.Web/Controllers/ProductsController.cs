using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Models;
using OrderFood.Infrastructure;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : BaseController
    {
        private readonly DBContext _context;

        public ProductsController(DBContext context)
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
        public async Task<IActionResult> Create(ProductModel model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var product = new Product()
                {
                    Title = model.Title,
                    Subtitle = model.Subtitle,
                    Description = model.Description,
                    Price = model.Price ?? 0,
                };

                if (image != null)
                {
                    // var extension = Path.GetExtension(image.FileName);
                    var fileName = $"{Guid.NewGuid()}-{image.FileName}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.ImageUrl = $"/img/{fileName}";
                }

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
        public async Task<IActionResult> Edit(ProductModel model, long[] categoryIds, IFormFile image)
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
                // product.ImageUrl = model.ImageUrl;

                product.ProductCategories = categoryIds.Select(categoryId => new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                }).ToList();

                if (image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/img/"))
                    {
                        var fileNameForDeleteImage = product.ImageUrl.Substring(5);
                        var deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameForDeleteImage);
                        if (System.IO.File.Exists(deletePath))
                        {
                            System.IO.File.Delete(deletePath);
                        }
                    }

                    var fileName = $"{Guid.NewGuid()}-{image.FileName}";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    product.ImageUrl = $"/img/{fileName}";
                }

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

        public async Task<IActionResult> RemoveImage(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/img/"))
            {
                var fileNameForDeleteImage = product.ImageUrl.Substring(5);
                var deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameForDeleteImage);
                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }
            }

            product.ImageUrl = null;

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Products", new { id = product.Id });
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
