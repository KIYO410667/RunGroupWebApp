using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
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
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAzureBlobService _azureBlobService;

        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor,
            IAzureBlobService azureBlobService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _azureBlobService = azureBlobService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dashboardVM = new DashboardViewModel()
            {
                Clubs = await _dashboardRepository.GetAllUserClub(),
                appUser = await _dashboardRepository.GetUserById(userId)
            };

            return View(dashboardVM);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _dashboardRepository.GetUserById(userId);
            if (currentUser == null) { return View("Error"); }

            var editUserVM = new EditUserProfileViewModel
            {
                Id = userId,
                UserName = currentUser.UserName,
                Bio = currentUser.Bio,
                ProfilePhotoUrl = currentUser.ProfilePhotoUrl,
                Address = currentUser.Address,
            };
            return View(editUserVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "輸入失敗，請重新嘗試");
                return View(editUserVM);
            }

            // 1. retrieve the current user
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _dashboardRepository.GetUserById(userId);
            if (currentUser == null) { return View("Error"); }

            // 2. Try to delete the existing photo, considering the case where it might not exist
            if (!string.IsNullOrEmpty(currentUser.ProfilePhotoUrl) && editUserVM.ProfilePhotoFile != null)
            {
                try
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(currentUser.ProfilePhotoUrl);
                }
                catch (Exception ex)
                {
                    // Log the exception, but continue with the update process
                    // The photo might not exist, or there might be other issues
                    ModelState.AddModelError(string.Empty, "圖片刪除失敗");
                    return View(editUserVM);
                }
            }

            //3. Update other user's properties
            currentUser.Bio = editUserVM.Bio;
            currentUser.Address.City = editUserVM.Address.City;
            currentUser.Address.Street = editUserVM.Address.Street;
            currentUser.UserName = editUserVM.UserName;

            //4. Handle the new image upload
            if (editUserVM.ProfilePhotoFile != null)
            {
                using (var stream = editUserVM.ProfilePhotoFile.OpenReadStream())
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(editUserVM.ProfilePhotoFile.FileName);
                    var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                    if (blobUrl == null)
                    {
                        ModelState.AddModelError("Image", "Photo upload failed");
                        return View(editUserVM);
                    }
                    currentUser.ProfilePhotoUrl = blobUrl; //only change the Image URL
                }
            }

            // 4. Update the model
            _dashboardRepository.Update(currentUser);

            return RedirectToAction("Index");
        }
    }
}
