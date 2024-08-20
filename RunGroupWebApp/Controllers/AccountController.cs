using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

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
        public IActionResult Login()
        {
            var loginVM = new LoginViewModel(); // Replace with your actual ViewModel initialization logic
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (!ModelState.IsValid) 
            {
                return View(LoginVM); 
            }

            var result = await _signInManager.PasswordSignInAsync(LoginVM.Email, LoginVM.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(LoginVM);
        }
    }
}
