using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Domain.Identity.Models;
using OrderFood.Web.Models;
using OrderFood.Web.Services;

namespace OrderFood.Web.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, WebBaseManager webBaseManager) : base(webBaseManager)
        {
            _emailSender = emailSender;
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

            // var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            // if (!emailConfirmed)
            // {
            //     SetFlash(FlashMessageType.Warning, "Please confirm your account with the link sent to your e-mail address.");
            //     return View(loginModel);
            // }

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

            var user = new ApplicationUser
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                // try
                // {
                //     var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //     var url = Url.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token = token });

                //     await _emailSender.SendEmailAsync(user.Email, "Confirm your account", $@"
                //         Hi, <strong>{user.FirstName}</strong>.

                //         Welcome to OrderFood.

                //         Please verify your account at this <a href='https://localhost:5001{url}'>link</a>.
                //     ");
                // }
                // catch (System.Exception)
                // {

                // }

                SetFlash(FlashMessageType.Success, $"Welcome {registerModel.FirstName}. Your registration was successful and confirmation email has been sent your mail address. If you have not received the email, you can resend it by clicking this link.");

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

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                SetFlash(FlashMessageType.Danger, "Invalid token.");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                SetFlash(FlashMessageType.Danger, "There is no such user");
                return View();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                SetFlash(FlashMessageType.Success, "Your account was successfully confirmed.");
                return View();
            }

            SetFlash(FlashMessageType.Danger, "Your account was not confirmed. Please try again.");

            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                SetFlash(FlashMessageType.Danger, "Email cannot be blank.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                SetFlash(FlashMessageType.Danger, $"No account associated with this email \"{email}\".");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Accounts", new { userId = user.Id, token = token });

            await _emailSender.SendEmailAsync(user.Email, "Reset password", $@"
                Hi, <strong>{user.FirstName}</strong>.

                You can use this <a href='https://localhost:5001{url}'>link</a> to reset your password.
            ");

            SetFlash(FlashMessageType.Success, "A link has been sent to your email address to reset your password.");

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                SetFlash(FlashMessageType.Danger, "Invalid token.");
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordModel { Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordModel);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);

            if (user == null)
            {
                SetFlash(FlashMessageType.Danger, $"No account associated with this email \"{resetPasswordModel.Email}\".");
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);

            if (result.Succeeded)
            {
                SetFlash(FlashMessageType.Success, "Your password was successfully changed.");
                return RedirectToAction("Login", "Accounts");
            }

            result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));

            return View(resetPasswordModel);
        }
    }
}
