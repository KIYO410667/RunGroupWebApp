using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class AppUserClubController : Controller
    {
        private readonly IAppUserClubRepository _appUserClubRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AppUserClubController> _logger;

        public AppUserClubController(IAppUserClubRepository appUserClubRepository,
                                     IHttpContextAccessor contextAccessor,
                                     ILogger<AppUserClubController> logger)
        {
            _appUserClubRepository = appUserClubRepository;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to add user to club with ID: {id}");
                var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userclub = await _appUserClubRepository.GetByIdAsync(id);
                if (userclub == null)
                {
                    var userClub = new AppUserClub { AppUserId = userId, ClubId = id };
                    _appUserClubRepository.Add(userClub);
                    _logger.LogInformation($"User {userId} added to club {id}");
                    return RedirectToAction("Index", "AppUserClub");
                }
                _logger.LogInformation($"User {userId} already in club {id}. Redirecting to Club Index.");
                return RedirectToAction("Index", "Club");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user to club with ID: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"Add operation completed for club ID: {id}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete user club with ID: {id}");
                var userClub = await _appUserClubRepository.GetByIdAsync(id);
                if (userClub == null)
                {
                    _logger.LogWarning($"User club with ID {id} not found for deletion");
                    return View("Error");
                }
                _appUserClubRepository.Delete(userClub);
                _logger.LogInformation($"User club with ID {id} deleted successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user club with ID: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"Delete operation completed for user club ID: {id}");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching all clubs for user");
                var clubs = await _appUserClubRepository.GetAllClubs();
                _logger.LogInformation($"Retrieved {clubs.Count()} clubs for user");
                return View(clubs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all clubs for user");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Index operation completed");
            }
        }
    }
}
