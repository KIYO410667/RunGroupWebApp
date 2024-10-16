using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Services
{
    public class ClubService : IClubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClubService> _logger;

        public ClubService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService,
            IHttpContextAccessor httpContextAccessor, ILogger<ClubService> logger)
        {
            _unitOfWork = unitOfWork;
            _azureBlobService = azureBlobService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<PaginatedList<ClubSummaryViewModel>> SearchClubs(string keyword, ClubCategory? category, City? city, int page, int pageSize)
        {
            try
            {
                var clubs = await _unitOfWork.Clubs.SearchClubsAsync(keyword, category, city, page, pageSize);
                var totalCount = await _unitOfWork.Clubs.GetSearchResultsCountAsync(keyword, category, city);
                return new PaginatedList<ClubSummaryViewModel>(clubs, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching clubs");
                throw;
            }
        }

        public async Task<ClubWithUsersViewModel> GetClubDetail(int id)
        {
            try
            {
                var club = await _unitOfWork.Clubs.GetClubWithAppUserById(id);
                if (club == null)
                {
                    return null;
                }
                var users = await _unitOfWork.Clubs.GetAllUsers(id);
                return new ClubWithUsersViewModel
                {
                    Club = club,
                    AppUsers = users
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting club details for id: {id}");
                throw;
            }
        }

        public async Task<Club> GetClubById(int id)
        {
            try
            {
                return await _unitOfWork.Clubs.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting club by id: {id}");
                throw;
            }
        }

        public async Task<bool> CreateClub(CreateClubViewModel clubVM)
        {
            try
            {
                using (var stream = clubVM.Image.OpenReadStream())
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(clubVM.Image.FileName);
                    var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);

                    Club club = new Club
                    {
                        Title = clubVM.Title,
                        Description = clubVM.Description,
                        Image = blobUrl,
                        AppUserId = clubVM.AppUserId,
                        Capacity = clubVM.Capacity,
                        Address = new Address
                        {
                            Street = clubVM.Address.Street,
                            City = clubVM.Address.City
                        },
                    };

                    _unitOfWork.Clubs.Add(club);
                    await _unitOfWork.CompleteAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating club");
                return false;
            }
        }

        public async Task<EditClubViewModel> GetClubForEdit(int id)
        {
            try
            {
                var club = await _unitOfWork.Clubs.GetById(id);
                if (club == null)
                {
                    return null;
                }
                return new EditClubViewModel
                {
                    Title = club.Title,
                    Description = club.Description,
                    URL = club.Image,
                    AddressId = club.AddressId,
                    Address = club.Address,
                    ClubCategory = club.ClubCategory,
                    Capacity = club.Capacity,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting club for edit, id: {id}");
                throw;
            }
        }

        public async Task<bool> UpdateClub(int id, EditClubViewModel clubVM)
        {
            try
            {
                var userClub = await _unitOfWork.Clubs.GetById(id);
                if (userClub == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(userClub.Image) && clubVM.Image != null)
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(userClub.Image);
                }

                userClub.Title = clubVM.Title;
                userClub.Description = clubVM.Description;
                userClub.AddressId = clubVM.AddressId;
                userClub.Address = clubVM.Address;
                userClub.ClubCategory = clubVM.ClubCategory;
                userClub.Capacity = clubVM.Capacity;

                if (clubVM.Image != null)
                {
                    using (var stream = clubVM.Image.OpenReadStream())
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(clubVM.Image.FileName);
                        var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                        if (blobUrl == null)
                        {
                            return false;
                        }
                        userClub.Image = blobUrl;
                    }
                }

                _unitOfWork.Clubs.Update(userClub);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating club, id: {id}");
                return false;
            }
        }

        public async Task<Club> GetClubForDelete(int id)
        {
            try
            {
                return await _unitOfWork.Clubs.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting club for delete, id: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteClub(int id)
        {
            try
            {
                var club = await _unitOfWork.Clubs.GetByIdIncludeAppUserClub(id);
                if (club == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(club.Image))
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(club.Image);
                }

                _unitOfWork.Clubs.Delete(club);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting club, id: {id}");
                return false;
            }
        }
    }
}
