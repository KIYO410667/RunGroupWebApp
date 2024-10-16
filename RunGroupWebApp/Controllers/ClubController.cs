using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces.IReposiotry;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubService _clubService;
        private readonly ILogger<ClubController> _logger;

        public ClubController(IClubService clubService, ILogger<ClubController> logger)
        {
            _clubService = clubService;
            _logger = logger;
        }

        private const int PageSize = 9; // 3x3 grid
        public async Task<IActionResult> Index(string keyword, ClubCategory? category, City? city, int page = 1)
        {
            try
            {
                _logger.LogInformation($"Index action called with keyword: {keyword}, category: {category}, city: {city}, page: {page}");

                ViewData["Keyword"] = keyword;
                ViewData["Category"] = category?.ToString();
                ViewData["City"] = city?.ToString();

                var model = await _clubService.SearchClubs(keyword, category, city, page, PageSize);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Index action");
                return View("Error");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                _logger.LogInformation($"Detail action called for club id: {id}");
                var clubUsers = await _clubService.GetClubDetail(id);
                if (clubUsers == null)
                {
                    _logger.LogWarning($"Club not found for id: {id}");
                    return View("Error");
                }
                return View(clubUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Detail action for club id: {id}");
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Create action called");
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var clubVM = new CreateClubViewModel { AppUserId = userId };
                return View(clubVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Create action");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            try
            {
                _logger.LogInformation("Create POST action called");
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState in Create POST action");
                    return View(clubVM);
                }

                var result = await _clubService.CreateClub(clubVM);
                if (result)
                {
                    _logger.LogInformation("New club created successfully");
                    return RedirectToAction("Index", new { message = "Photo uploaded successfully!" });
                }
                else
                {
                    _logger.LogWarning("Failed to create new club");
                    ModelState.AddModelError(string.Empty, "Failed to create club. Please try again.");
                    return View(clubVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Create POST action");
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation($"Edit action called for club id: {id}");
                var clubVM = await _clubService.GetClubForEdit(id);
                if (clubVM == null)
                {
                    _logger.LogWarning($"Club not found for edit, id: {id}");
                    return View("Error");
                }
                return View(clubVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Edit action for club id: {id}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            try
            {
                _logger.LogInformation($"Edit POST action called for club id: {id}");
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState in Edit POST action");
                    ModelState.AddModelError(string.Empty, "輸入失敗，請重新嘗試");
                    return View(clubVM);
                }

                var result = await _clubService.UpdateClub(id, clubVM);
                if (result)
                {
                    _logger.LogInformation($"Club updated successfully, id: {id}");
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning($"Failed to update club, id: {id}");
                    ModelState.AddModelError(string.Empty, "Failed to update club. Please try again.");
                    return View(clubVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Edit POST action for club id: {id}");
                return View("Error");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Delete action called for club id: {id}");
                var club = await _clubService.GetClubForDelete(id);
                if (club == null)
                {
                    _logger.LogWarning($"Club not found for delete, id: {id}");
                    return View("Error");
                }
                return View(club);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Delete action for club id: {id}");
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteConfirmed action called for club id: {id}");
                var result = await _clubService.DeleteClub(id);
                if (result)
                {
                    _logger.LogInformation($"Club deleted successfully, id: {id}");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning($"Failed to delete club, id: {id}");
                    ModelState.AddModelError(string.Empty, "刪除俱樂部時發生錯誤。可能是因為此俱樂部仍有關聯的數據。");
                    var club = await _clubService.GetClubById(id);
                    return View(club);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred in DeleteConfirmed action for club id: {id}");
                return View("Error");
            }
        }
    }
}
