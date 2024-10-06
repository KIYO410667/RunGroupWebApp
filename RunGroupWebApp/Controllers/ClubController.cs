using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClubController> _logger;

        public ClubController(IClubRepository clubRepository, IAzureBlobService azureBlobService,
            IHttpContextAccessor httpContextAccessor, ILogger<ClubController> logger)
        {
            _clubRepository = clubRepository;
            _azureBlobService = azureBlobService;
            _httpContextAccessor = httpContextAccessor;
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

                var clubs = await _clubRepository.SearchClubsAsync(keyword, category, city, page, PageSize);
                var totalCount = await _clubRepository.GetSearchResultsCountAsync(keyword, category, city);

                var model = new PaginatedList<ClubSummaryViewModel>(clubs, totalCount, page, PageSize);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Index action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Index action completed");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                _logger.LogInformation($"Detail action called for club id: {id}");
                Club club = await _clubRepository.GetClubWithAppUserById(id);
                if (club == null)
                {
                    _logger.LogWarning($"Club not found for id: {id}");
                    return View("Error");
                }
                List<AppUser> users = await _clubRepository.GetAllUsers(id);
                ClubWithUsersViewModel clubUsers = new ClubWithUsersViewModel()
                {
                    Club = club,
                    AppUsers = users
                };
                return View(clubUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Detail action for club id: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"Detail action completed for club id: {id}");
            }
        }

        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Create action called");
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var clubVM = new CreateClubViewModel { AppUserId = userId };
                return View(clubVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Create action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Create action completed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel ClubVM)
        {
            try
            {
                _logger.LogInformation("Create POST action called");
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState in Create POST action");
                    return View(ClubVM);
                }

                using (var stream = ClubVM.Image.OpenReadStream())
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ClubVM.Image.FileName);
                    var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);

                    Club club = new Club
                    {
                        Title = ClubVM.Title,
                        Description = ClubVM.Description,
                        Image = blobUrl,
                        AppUserId = ClubVM.AppUserId,
                        Address = new Address
                        {
                            Street = ClubVM.Address.Street,
                            City = ClubVM.Address.City
                        },
                    };

                    _clubRepository.Add(club);

                    _logger.LogInformation($"New club created with id: {club.Id}");
                    return RedirectToAction("Index", new { message = "Photo uploaded successfully!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Create POST action");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation("Create POST action completed");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                _logger.LogInformation($"Edit action called for club id: {id}");
                var club = await _clubRepository.GetById(id);
                if (club == null)
                {
                    _logger.LogWarning($"Club not found for edit, id: {id}");
                    return View("Error");
                }
                var clubVM = new EditClubViewModel
                {
                    Title = club.Title,
                    Description = club.Description,
                    URL = club.Image,
                    AddressId = club.AddressId,
                    Address = club.Address,
                    ClubCategory = club.ClubCategory,
                };
                return View(clubVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Edit action for club id: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"Edit action completed for club id: {id}");
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

                var userClub = await _clubRepository.GetById(id);

                if (userClub == null)
                {
                    _logger.LogWarning($"Club not found for edit, id: {id}");
                    return View("Error");
                }

                if (!string.IsNullOrEmpty(userClub.Image) && clubVM.Image != null)
                {
                    try
                    {
                        await _azureBlobService.DeletePhotoByUrlAsync(userClub.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to delete existing photo for club id: {id}");
                        ModelState.AddModelError(string.Empty, "圖片刪除失敗");
                        return View(clubVM);
                    }
                }

                userClub.Title = clubVM.Title;
                userClub.Description = clubVM.Description;
                userClub.AddressId = clubVM.AddressId;
                userClub.Address = clubVM.Address;
                userClub.ClubCategory = clubVM.ClubCategory;

                if (clubVM.Image != null)
                {
                    using (var stream = clubVM.Image.OpenReadStream())
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(clubVM.Image.FileName);
                        var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                        if (blobUrl == null)
                        {
                            _logger.LogError($"Failed to upload new photo for club id: {id}");
                            ModelState.AddModelError("Image", "Photo upload failed");
                            return View(clubVM);
                        }
                        userClub.Image = blobUrl;
                    }
                }

                _clubRepository.Update(userClub);

                _logger.LogInformation($"Club updated successfully, id: {id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Edit POST action for club id: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"Edit POST action completed for club id: {id}");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Delete action called for club id: {id}");
                var club = await _clubRepository.GetById(id);
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
            finally
            {
                _logger.LogInformation($"Delete action completed for club id: {id}");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteConfirmed action called for club id: {id}");
                var club = await _clubRepository.GetByIdIncludeAppUserClub(id);
                if (club == null)
                {
                    _logger.LogWarning($"Club not found for delete confirmation, id: {id}");
                    return View("Error");
                }

                // Delete the associated image from Azure Blob Storage
                if (!string.IsNullOrEmpty(club.Image))
                {
                    try
                    {
                        await _azureBlobService.DeletePhotoByUrlAsync(club.Image);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception, but continue with the deletion process
                        // The photo might not exist, or there might be other issues
                        _logger.LogError(ex, $"Failed to delete photo for club id: {id}");
                        ModelState.AddModelError(string.Empty, "圖片刪除失敗，但會繼續刪除俱樂部資料");
                    }
                }

                try
                {
                    // Delete the club from the database
                    _clubRepository.Delete(club);

                    _logger.LogInformation($"Club deleted successfully, id: {id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception
                    _logger.LogError(ex, $"Error deleting club {id}");
                    ModelState.AddModelError(string.Empty, "刪除俱樂部時發生錯誤。可能是因為此俱樂部仍有關聯的數據。");
                    return View(club);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred in DeleteConfirmed action for club id: {id}");
                return View("Error");
            }
            finally
            {
                _logger.LogInformation($"DeleteConfirmed action completed for club id: {id}");
            }
        }
    }
}
