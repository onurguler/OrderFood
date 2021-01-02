using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Domain.Identity;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login(string returnUrl = null)
        {
            var loginModel = new LoginModel { ReturnUrl = returnUrl };
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Incorrect email or password.");
                return View(loginModel);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, true, true);

            if (result.Succeeded)
            {
                SetFlash(FlashMessageType.Success, $"Welcome {user.FirstName}. You are successfully logged in.");

                if (!string.IsNullOrEmpty(loginModel.ReturnUrl))
                {
                    return Redirect(loginModel.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Too many login attempts. Your account was locked for 5 minutes.");
            }

            ModelState.AddModelError("Email", "Incorrect email or password");

            return View(loginModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var user = new User
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                SetFlash(FlashMessageType.Success, $"Welcome {registerModel.FirstName}. Your registration was successful.");
                return RedirectToAction("Login", "Accounts");
            }

            result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));

            return View(registerModel);
        }

        [Authorize]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostLogout()
        {
            await _signInManager.SignOutAsync();

            SetFlash(FlashMessageType.Success, "You are successfully logged out.");

            return RedirectToAction("Login", "Accounts");
        }
    }
}
