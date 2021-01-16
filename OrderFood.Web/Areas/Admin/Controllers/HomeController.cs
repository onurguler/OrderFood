using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Web.Controllers;
using OrderFood.Web.Services;

namespace OrderFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : BaseController
    {
        public HomeController(WebBaseManager webBaseManager) : base(webBaseManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
