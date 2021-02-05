using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Domain.Dto;
using OrderFood.Web.Controllers;
using OrderFood.Web.Models;
using OrderFood.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OrderFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : BaseController
    {
        public ProductsController(WebBaseManager webBaseManager) : base(webBaseManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(string search, string sort, string order, int offset, int limit)
        {
            var products = await WebBaseManager.ApplicationBaseManager.ProductManager.GetBsTableResult(search, sort, order, offset, limit);

            var total = 0;

            if (!string.IsNullOrWhiteSpace(search))
            {
                total = products.Count();
            }
            else
            {
                total = await WebBaseManager.ApplicationBaseManager.ProductManager.CountProducts();
            }

            return Json(new { rows = products, total });
        }

        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var product = await WebBaseManager.ApplicationBaseManager.ProductManager.CreateProduct(model, image);
                SetFlash(FlashMessageType.Success, $"Product \"{product.Title}\" was successfully created.");
                return Json(new { data = product });
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            return PartialView(model);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var product = await WebBaseManager.ApplicationBaseManager.ProductManager.GetProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            // ViewBag.Categories = await WebBaseManager.ApplicationBaseManager.CategoryManager.GetCategoriesList();

            return PartialView(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model, IFormFile image, long[] categoryIds)
        {
            if (ModelState.IsValid)
            {
                var result = await WebBaseManager.ApplicationBaseManager.ProductManager.EditProduct(model, image);

                if (result == null)
                {
                    return NotFound();
                }

                SetFlash(FlashMessageType.Success, $"Product \"{result.Title}\" was successfully updated.");

                return Json(new { data = result });
            }

            SetFlash(FlashMessageType.Danger, "Please review information that you have entered.");

            // ViewBag.Categories = await WebBaseManager.ApplicationBaseManager.CategoryManager.GetCategoriesList();

            return PartialView(model);
        }

        public async Task<IActionResult> RemoveImage(long id)
        {
            var result = await WebBaseManager.ApplicationBaseManager.ProductManager.RemoveImage(id);

            if (!result)
            {
                return NotFound();
            }

            SetFlash(FlashMessageType.Success, "Product image was successfully removed.");

            return RedirectToAction("Edit", new { id });
        }

        public async Task<IActionResult> Delete(long id)
        {
            var result = await WebBaseManager.ApplicationBaseManager.ProductManager.GetProduct(id);

            if (result == null)
            {
                return NotFound();
            }

            return PartialView(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductDto model)
        {
            var result = await WebBaseManager.ApplicationBaseManager.ProductManager.DeleteProduct(model.Id);

            if (!result)
            {
                return NotFound();
            }

            SetFlash(FlashMessageType.Success, $"Product \"{model.Id}\" was successfully deleted.");

            return NoContent();
        }
    }
}
