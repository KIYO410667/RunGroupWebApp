using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Login(string errorMessage = null)
        {
            var loginVM = new LoginViewModel()
            {
            }; // Replace with your actual ViewModel initialization logic
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(LoginVM);
            }

            // 查找用户
            var user = await _userManager.FindByEmailAsync(LoginVM.Email);

            if (user == null)
            {
                // 用户不存在，返回错误信息
                //ViewBag.ErrorMessage = "Invalid login attempt. Email not found.";
                ModelState.AddModelError(string.Empty, "Invalid login attempt. 無效的登入嘗試");
                return View(LoginVM);
            }

            // 验证密码
            var passwordCheck = await _userManager.CheckPasswordAsync(user, LoginVM.Password);

            if (!passwordCheck)
            {
                // 密码不正确，返回错误信息
                //ViewBag.ErrorMessage = "Invalid login attempt. Incorrect password.";
                ModelState.AddModelError(string.Empty, "Invalid login attempt. 無效的登入嘗試");
                return View(LoginVM);
            }

            // 如果Email和密码都正确，则登录用户
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Register()
        {
            var registerVM = new RegisterViewModel();
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "此電子郵件已註冊，請重試";
                return View(registerVM);
            }

            var user = new AppUser { UserName = registerVM.Email, Email = registerVM.Email };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Code);
            }
            return View(registerVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // New method to handle the callback from Google
        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                // Handle the "access_denied" error specifically
                if (remoteError == "access_denied")
                {
                    return RedirectToAction("Login", new { errorMessage = "You canceled the Google login process. Please try again or use another login method." });
                }
                // Handle other remote errors
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // This can happen if the user canceled the login process
                return RedirectToAction("Login", new { errorMessage = "There was an issue with the external login. Please try again." });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new AppUser { UserName = email, Email = email };
                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, UserRoles.User);
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
