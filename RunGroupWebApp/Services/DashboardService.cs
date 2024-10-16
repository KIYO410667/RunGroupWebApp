using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureBlobService _azureBlobService;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _azureBlobService = azureBlobService;
            _logger = logger;
        }

        public async Task<DashboardViewModel> GetDashboardInformation()
        {
            _logger.LogInformation("Fetching dashboard information");
            var dashboardVM = new DashboardViewModel()
            {
                Clubs = await _unitOfWork.Dashboard.GetAllUserClub(),
                appUser = await _unitOfWork.Dashboard.GetUserById()
            };
            _logger.LogInformation("Dashboard information retrieved successfully");
            return dashboardVM;
        }

        public async Task<EditUserProfileViewModel> GetUserProfileForEdit()
        {
            _logger.LogInformation("Retrieving user profile for editing");
            var currentUser = await _unitOfWork.Dashboard.GetUserById();
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when attempting to edit profile");
                return null;
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
            return editUserVM;
        }

        public async Task<bool> UpdateUserProfile(EditUserProfileViewModel editUserVM)
        {
            _logger.LogInformation("Attempting to update user profile");
            var currentUser = await _unitOfWork.Dashboard.GetUserById();
            if (currentUser == null)
            {
                _logger.LogWarning("User not found when updating profile");
                return false;
            }

            try
            {
                _logger.LogInformation("Updating user profile properties");
                if (currentUser.Address == null)
                {
                    currentUser.Address = new Address();
                }
                currentUser.Bio = editUserVM.Bio;
                currentUser.Address.City = editUserVM.Address.City;
                currentUser.Address.Street = editUserVM.Address.Street;
                currentUser.UserName = editUserVM.UserName;

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
                            return false;
                        }
                        else
                        {
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
                                    return false;
                                }
                            }
                            currentUser.ProfilePhotoUrl = blobUrl;
                        }
                        _logger.LogInformation("New profile photo uploaded successfully");
                    }
                }

                _logger.LogInformation("Updating user profile in repository");
                _unitOfWork.Users.Update(currentUser);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("User profile updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile");
                return false;
            }
        }
    }
}
