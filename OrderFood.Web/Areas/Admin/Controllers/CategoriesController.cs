using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Web.Controllers;
using OrderFood.Web.Services;

namespace OrderFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController
    {
        public CategoriesController(WebBaseManager webBaseManager) : base(webBaseManager)
        {

        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var categories = await WebBaseManager.ApplicationBaseManager.CategoryManager.GetCategoriesList();
            return Json(new { data = categories });
        }
    }
}