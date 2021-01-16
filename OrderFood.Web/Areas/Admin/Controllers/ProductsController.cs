using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Domain.Dto;
using OrderFood.Web.Controllers;
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
    }
}
