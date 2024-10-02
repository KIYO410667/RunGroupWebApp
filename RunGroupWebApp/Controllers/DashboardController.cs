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
        private readonly IAzureBlobService _azureBlobService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardRepository dashboardRepository,
            IAzureBlobService azureBlobService,
            ILogger<DashboardController> logger)
        {
            _dashboardRepository = dashboardRepository;
            _azureBlobService = azureBlobService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching dashboard information");
            var dashboardVM = new DashboardViewModel()
            {
                Clubs = await _dashboardRepository.GetAllUserClub(),
                appUser = await _dashboardRepository.GetUserById()
            };
            _logger.LogInformation("Dashboard information retrieved successfully");
            return View(dashboardVM);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            _logger.LogInformation("Retrieving user profile for editing");
            var currentUser = await _dashboardRepository.GetUserById();
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when attempting to edit profile");
                return View("Error");
            }
            var editUserVM = new EditUserProfileViewModel
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                Bio = currentUser.Bio,
                ProfilePhotoUrl = currentUser.ProfilePhotoUrl,
                Address = currentUser.Address,
            };
            _logger.LogInformation("User profile retrieved for editing");
            return View(editUserVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserVM)
        {
            _logger.LogInformation("Attempting to update user profile");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when updating user profile");
                return View(editUserVM);
            }

            // retrieve the current user
            var currentUser = await _dashboardRepository.GetUserById();
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when updating profile");
                return View("Error");
            }

            try
            {
                // Update other user's properties
                _logger.LogInformation("Updating user profile properties");
                if (currentUser.Address == null)
                {
                    currentUser.Address = new Address();
                }
                currentUser.Bio = editUserVM.Bio;
                currentUser.Address.City = editUserVM.Address.City;
                currentUser.Address.Street = editUserVM.Address.Street;
                currentUser.UserName = editUserVM.UserName;

                // Handle the new image upload
                if (editUserVM.ProfilePhotoFile != null)
                {
                    _logger.LogInformation("Uploading new profile photo");
                    using (var stream = editUserVM.ProfilePhotoFile.OpenReadStream())
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(editUserVM.ProfilePhotoFile.FileName);
                        var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                        if (blobUrl == null)
                        {
                            _logger.LogWarning("Photo upload failed");
                            ModelState.AddModelError("Image", "Photo upload failed");
                            return View(editUserVM);
                        }
                        else
                        {
                            // Try to delete the existing photo
                            if (!string.IsNullOrEmpty(currentUser.ProfilePhotoUrl))
                            {
                                try
                                {
                                    _logger.LogInformation("Attempting to delete existing profile photo");
                                    await _azureBlobService.DeletePhotoByUrlAsync(currentUser.ProfilePhotoUrl);
                                    _logger.LogInformation("Existing profile photo deleted successfully");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error occurred while deleting existing profile photo");
                                    return View(editUserVM);
                                }
                            }
                            currentUser.ProfilePhotoUrl = blobUrl;
                        }
                        _logger.LogInformation("New profile photo uploaded successfully");
                    }
                }

                // 4. Update the model
                _logger.LogInformation("Updating user profile in repository");
                _dashboardRepository.Update(currentUser);
                _logger.LogInformation("User profile updated successfully");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting existing profile photo");
                return View(editUserVM);
            }
            
        }
    }
}
