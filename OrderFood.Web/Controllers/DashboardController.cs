using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Infrastructure.Data;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly OrderFoodContext _context;

        public DashboardController(OrderFoodContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Products(string searchString, string sortOrder)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Title.ToLower().Contains(searchString.ToLower())
                    || p.Subtitle.ToLower().Contains(searchString.ToLower())
                    || p.Description.ToLower().Contains(searchString.ToLower()))
                    .AsQueryable();
                ViewData["SearchString"] = searchString;
            }

            var products = await productsQuery.AsNoTracking().ToListAsync();

            var viewModel = new ProductListViewModel() { Products = products };
            return View(viewModel);
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            var viewModel = new CategoryListViewModel { Categories = categories };
            return View(viewModel);
        }
    }
}
