using Microsoft.AspNetCore.Identity;
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

        public AccountController(UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager
            , ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _logger.LogInformation("AccountController instantiated");
        }

        public async Task<IActionResult> Login()
        {
            _logger.LogInformation("Login GET action started");
            try
            {
                var loginVM = new LoginViewModel();
                _logger.LogInformation("Login view model created");
                return View(loginVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login GET action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Login GET action completed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            _logger.LogInformation("Login POST action started");
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in Login POST action");
                    return View(LoginVM);
                }

                _logger.LogInformation("Attempting to find user by email: {Email}", LoginVM.Email);
                var user = await _userManager.FindByEmailAsync(LoginVM.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: User not found for email {Email}", LoginVM.Email);
                    ModelState.AddModelError("Email", "Invalid login attempt. Please try again.");
                    return View(LoginVM);
                }

                _logger.LogInformation("Checking password for user: {UserId}", user.Id);
                var passwordCheck = await _userManager.CheckPasswordAsync(user, LoginVM.Password);

                if (!passwordCheck)
                {
                    _logger.LogWarning("Login attempt failed: Invalid password for user {UserId}", user.Id);
                    ModelState.AddModelError("Password", "Invalid login attempt. Please try again.");
                    return View(LoginVM);
                }

                _logger.LogInformation("Password check successful for user {UserId}", user.Id);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User {UserId} signed in successfully", user.Id);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Login POST action");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(LoginVM);
            }
            finally
            {
                _logger.LogInformation("Login POST action completed");
            }
        }

        public IActionResult Register()
        {
            _logger.LogInformation("Register GET action started");
            try
            {
                var registerVM = new RegisterViewModel();
                _logger.LogInformation("Register view model created");
                return View(registerVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Register GET action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Register GET action completed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            _logger.LogInformation("Register POST action started");
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in Register POST action");
                    return View(registerVM);
                }

                _logger.LogInformation("Checking for existing user with email: {Email}", registerVM.Email);
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration attempt failed: Email {Email} already in use", registerVM.Email);
                    ModelState.AddModelError("Email", "此電子郵件已註冊，請重試");
                    return View(registerVM);
                }

                _logger.LogInformation("Creating new user with email: {Email}", registerVM.Email);
                var user = new AppUser { UserName = registerVM.Email, Email = registerVM.Email };

                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} created successfully", user.Id);
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                    _logger.LogInformation("User {UserId} assigned to role: {Role}", user.Id, UserRoles.User);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("User creation error: {ErrorCode} - {ErrorDescription}", error.Code, error.Description);
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
            finally
            {
                _logger.LogInformation("Register POST action completed");
            }
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout action started");
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User signed out successfully");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Logout action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Logout action completed");
            }
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            _logger.LogInformation("ExternalLogin action started for provider: {Provider}", provider);
            try
            {
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
                _logger.LogInformation("Configuring external authentication properties for provider: {Provider}", provider);
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ExternalLogin action for provider: {Provider}", provider);
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("ExternalLogin action completed for provider: {Provider}", provider);
            }
        }

        public async Task<IActionResult> ExternalLoginCallback(string remoteError = null)
        {
            _logger.LogInformation("ExternalLoginCallback action started");
            try
            {
                if (remoteError != null)
                {
                    if (remoteError == "access_denied")
                    {
                        _logger.LogWarning("User canceled the Google login process.");
                        return RedirectToAction("AccessDenied", new { errorMessage = "You canceled the Google login process. Please try again or use another login method." });
                    }
                    _logger.LogError("Error from external provider: {RemoteError}", remoteError);
                    return View("AccessDenied");
                }

                _logger.LogInformation("Retrieving external login info");
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogError("External login info is null.");
                    return RedirectToAction("Login", new { errorMessage = "There was an issue with the external login. Please try again." });
                }

                _logger.LogInformation("Attempting to sign in user with external login provider: {LoginProvider}", info.LoginProvider);
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var photoUrl = info.Principal.FindFirstValue("picture");

                _logger.LogInformation("External login attempt for email: {Email}", email);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in successfully with external provider");
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null && user.ProfilePhotoUrl != photoUrl)
                    {
                        _logger.LogInformation("Updating profile photo for user: {UserId}", user.Id);
                        user.ProfilePhotoUrl = photoUrl;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (updateResult.Succeeded)
                        {
                            _logger.LogInformation("User profile photo updated successfully for user: {UserId}", user.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update user profile photo for user: {UserId}", user.Id);
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (email != null)
                    {
                        _logger.LogInformation("Checking for existing user with email: {Email}", email);
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            _logger.LogInformation("Creating new user for email: {Email}", email);
                            user = new AppUser
                            {
                                UserName = email,
                                Email = email,
                                ProfilePhotoUrl = photoUrl
                            };
                            var createResult = await _userManager.CreateAsync(user);
                            if (!createResult.Succeeded)
                            {
                                _logger.LogError("Failed to create user account for email: {Email}", email);
                                ModelState.AddModelError(string.Empty, "Error creating user account.");
                                return View("Login");
                            }
                            await _userManager.AddToRoleAsync(user, UserRoles.User);
                            _logger.LogInformation("New user account created and assigned role for user: {UserId}", user.Id);
                        }
                        else
                        {
                            if (user.ProfilePhotoUrl != photoUrl)
                            {
                                _logger.LogInformation("Updating profile photo for existing user: {UserId}", user.Id);
                                user.ProfilePhotoUrl = photoUrl;
                                var updateResult = await _userManager.UpdateAsync(user);
                                if (!updateResult.Succeeded)
                                {
                                    _logger.LogError("Failed to update existing user's profile photo for user: {UserId}", user.Id);
                                }
                            }
                            _logger.LogInformation("Existing user found, updated profile photo if necessary for user: {UserId}", user.Id);
                        }

                        _logger.LogInformation("Adding external login to user account: {UserId}", user.Id);
                        await _userManager.AddLoginAsync(user, info);

                        _logger.LogInformation("Signing in user: {UserId}", user.Id);
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        _logger.LogInformation("User signed in successfully after linking external login: {UserId}", user.Id);
                        return RedirectToAction("Index", "Home");
                    }

                    _logger.LogError("Email not found from external login.");
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the external login process.");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("ExternalLoginCallback action completed");
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied page requested");
            try
            {
                return View();
            }
            finally
            {
                _logger.LogInformation("AccessDenied action completed");
            }
        }
    }
}
