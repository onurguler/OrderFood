using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Models;
using OrderFood.Infrastructure;
using OrderFood.Web.Models;
using OrderFood.Web.Services;

namespace OrderFood.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController
    {
        private readonly DBContext _context;

        public CategoriesController(DBContext context, WebBaseManager webBaseManager) : base(webBaseManager)
        {
            this._context = context;

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category { Title = model.Title };

                _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                SetFlash(FlashMessageType.Success, $"Category \"{model.Title}\" was successfully created.");

                return RedirectToAction("Categories", "Dashboard");
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var category = await _context.Categories.Include(c => c.ProductCategories).ThenInclude(pc => pc.Product).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryModel
            {
                Id = category.Id,
                Title = category.Title,
                Products = category.ProductCategories.Select(pc => pc.Product).Select(p => new ProductModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(model.Id);

                if (category == null)
                {
                    return NotFound();
                }

                category.Title = model.Title;

                _context.Entry(category).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                SetFlash(FlashMessageType.Success, $"Category \"{model.Title}\" was successfully updated.");

                return RedirectToAction("Categories", "Dashboard");
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryModel { Id = category.Id, Title = category.Title };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CategoryModel model)
        {
            var category = await _context.Categories.FindAsync(model.Id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            SetFlash(FlashMessageType.Success, $"Product \"{category.Title}\" was successfully deleted.");

            return RedirectToAction("Categories", "Dashboard");
        }
    }
}
