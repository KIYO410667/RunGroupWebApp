﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager
            , ILogger<AccountController> logger
            , IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Login()
        {
            try
            {
                var loginVM = new LoginViewModel();
                return View(loginVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login GET action");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(LoginVM);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(LoginVM.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Invalid login attempt. Please try again.");
                    return View(LoginVM);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, LoginVM.Password);

                if (!passwordCheck)
                {
                    ModelState.AddModelError("Password", "Invalid login attempt. Please try again.");
                    return View(LoginVM);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login POST action");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(LoginVM);
            }
        }


        public IActionResult Register()
        {
            try
            {
                var registerVM = new RegisterViewModel();
                return View(registerVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register GET action");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "此電子郵件已註冊，請重試");
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
                    ModelState.AddModelError("string.Empty", error.Code);
                }
                return View(registerVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register POST action");
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                return View(registerVM);
            }

        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Logout action");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            try
            {
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ExternalLogin action");
                return View("Error");
            }
        }

        // New method to handle the callback from Google
        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null)
        {
            if (remoteError != null)
            {
                if (remoteError == "access_denied")
                {
                    return RedirectToAction("AccessDenied", new { errorMessage = "You canceled the Google login process. Please try again or use another login method." });
                }
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("AccessDenied");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login", new { errorMessage = "There was an issue with the external login. Please try again." });
            }

            // Attempt to sign in the user with the external login provider
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var photoUrl = info.Principal.FindFirstValue("picture");

            if (result.Succeeded)
            {
                // User has successfully logged in
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.ProfilePhotoUrl != photoUrl)
                {
                    // Update photo URL if it has changed
                    user.ProfilePhotoUrl = photoUrl;
                    await _userManager.UpdateAsync(user);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // If the user does not have an account, then we need to create one
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = email,
                            Email = email,
                            ProfilePhotoUrl = photoUrl
                        };
                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, "Error creating user account.");
                            return View("Login");
                        }
                        await _userManager.AddToRoleAsync(user, UserRoles.User);
                    }
                    else
                    {
                        // Existing user, update photo if necessary
                        if (user.ProfilePhotoUrl != photoUrl)
                        {
                            user.ProfilePhotoUrl = photoUrl;
                            await _userManager.UpdateAsync(user);
                        }
                    }

                    // Add the external login to the user account
                    await _userManager.AddLoginAsync(user, info);

                    // Sign in the user
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
