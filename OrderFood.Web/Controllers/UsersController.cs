using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Identity;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var roles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();

            var viewModel = new UserEditModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                UserRoles = userRoles,
            };

            ViewBag.Roles = roles;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditModel userModel, string[] rolesToAdd)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userModel.Id);

                if (user == null)
                {
                    return NotFound();
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                userModel.UserRoles = userRoles;

                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.UserName = userModel.UserName;
                user.Email = userModel.Email;
                user.EmailConfirmed = userModel.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    rolesToAdd = rolesToAdd ?? new string[] { };
                    var result1 = await _userManager.AddToRolesAsync(user, rolesToAdd.Except(userRoles).ToArray<string>());
                    var result2 = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(rolesToAdd).ToArray<string>());

                    bool success = true;

                    if (!result1.Succeeded)
                    {
                        success = false;
                        result1.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                    }

                    if (!result2.Succeeded)
                    {
                        success = false;
                        result2.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                    }

                    if (success)
                    {
                        SetFlash(FlashMessageType.Success, $"User \"{user.UserName}\" was successfully updated.");
                        return RedirectToAction("Users", "Dashboard");
                    }
                }
                else
                {
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                }
            }

            var roles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
            ViewBag.Roles = roles;

            SetFlash(FlashMessageType.Danger, "A problem occurred.");

            return View(userModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditModel
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserEditModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                SetFlash(FlashMessageType.Success, $"User \"{user.UserName}\" was successfully deleted.");
                return RedirectToAction("Users", "Dashboard");
            }
            else
            {
                SetFlash(FlashMessageType.Danger, "A problem occurred.");
                result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
            }

            return View(model);
        }
    }
}
