using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class AppUserClubController : Controller
    {
        private readonly IAppUserClubService _appUserClubService;
        private readonly ILogger<AppUserClubController> _logger;

        public AppUserClubController(IAppUserClubService appUserClubService, ILogger<AppUserClubController> logger)
        {
            _appUserClubService = appUserClubService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            try
            {
                var result = await _appUserClubService.AddUserToClub(id);
                if (result)
                {
                    return RedirectToAction("Index", "AppUserClub");
                }
                return RedirectToAction("Index", "Club");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user to club with ID: {id}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _appUserClubService.RemoveUserFromClub(id);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user club with ID: {id}");
                return View("Error");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var clubs = await _appUserClubService.GetAllUserClubs();
                return View(clubs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all clubs for user");
                return View("Error");
            }
        }
    }
}
