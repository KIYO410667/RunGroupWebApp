using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces.IReposiotry;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.Services;
using RunGroupWebApp.ViewModels;
using System.IO;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardVM = await _dashboardService.GetDashboardInformation();
                return View(dashboardVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dashboard information");
                return View("Error");
            }
        }

        public async Task<IActionResult> EditUserProfile()
        {
            try
            {
                var editUserVM = await _dashboardService.GetUserProfileForEdit();
                if (editUserVM == null)
                {
                    _logger.LogWarning("User profile not found for editing");
                    return View("Error");
                }
                return View(editUserVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user profile for editing");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when updating user profile");
                return View(editUserVM);
            }

            try
            {
                var result = await _dashboardService.UpdateUserProfile(editUserVM);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning("Failed to update user profile");
                    ModelState.AddModelError("", "Failed to update profile. Please try again.");
                    return View(editUserVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user profile");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(editUserVM);
            }
        }
    }
}
