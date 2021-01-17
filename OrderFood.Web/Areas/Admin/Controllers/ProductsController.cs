using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Domain.Dto;
using OrderFood.Web.Controllers;
using OrderFood.Web.Models;
using OrderFood.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : BaseController
    {
        public ProductsController(WebBaseManager webBaseManager) : base(webBaseManager)
        {
                
        }

        public async Task<IActionResult> Index(string searchString)
        {
            List<ProductDto> products = null;

            if (string.IsNullOrEmpty(searchString))
            {
                products = await WebBaseManager.ApplicationBaseManager.ProductManager.GetProductList();
            }
            else
            {
                products = await WebBaseManager.ApplicationBaseManager.ProductManager.SearchProducts(searchString);
                ViewBag.SearchString = searchString;
            }

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var product = await WebBaseManager.ApplicationBaseManager.ProductManager.CreateProduct(model, image);
                SetFlash(FlashMessageType.Success, $"Product \"{product.Title}\" was successfully created.");
                return RedirectToAction("Index");
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            return View(model);
        }
    }
}
