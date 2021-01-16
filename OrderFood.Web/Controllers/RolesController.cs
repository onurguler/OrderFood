using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Web.Models;
using OrderFood.Web.Services;

namespace OrderFood.Web.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager, WebBaseManager webBaseManager) : base(webBaseManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleModel.Name);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    SetFlash(FlashMessageType.Success, $"Role \"{roleModel.Name}\" was successfully created.");
                    return RedirectToAction("Roles", "Dashboard");
                }
                else
                {
                    SetFlash(FlashMessageType.Danger, "A problem occurred.");
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                }
            }

            return View(roleModel);
        }
    }
}
