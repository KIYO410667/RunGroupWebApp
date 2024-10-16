using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using System.Security.Claims;

namespace RunGroupWebApp.Services
{
    public class AppUserClubService : IAppUserClubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AppUserClubService> _logger;

        public AppUserClubService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, ILogger<AppUserClubService> logger)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<bool> AddUserToClub(int clubId)
        {
            try
            {
                _logger.LogInformation($"Attempting to add user to club with ID: {clubId}");
                var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userClub = await _unitOfWork.AppUserClubs.GetByIdAsync(clubId);

                if (userClub == null)
                {
                    var newUserClub = new AppUserClub { AppUserId = userId, ClubId = clubId };
                    _unitOfWork.AppUserClubs.Add(newUserClub);
                    //var club = await _unitOfWork.Clubs.GetById(clubId);
                    //club.ParticipantsCount += 1;
                    //_unitOfWork.Clubs.Update(club);
                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation($"User {userId} added to club {clubId}");
                    return true;
                }

                _logger.LogInformation($"User {userId} already in club {clubId}.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user to club with ID: {clubId}");
                throw;
            }
        }

        public async Task<bool> RemoveUserFromClub(int userClubId)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete user club with ID: {userClubId}");
                var userClub = await _unitOfWork.AppUserClubs.GetByIdAsync(userClubId);

                if (userClub == null)
                {
                    _logger.LogWarning($"User club with ID {userClubId} not found for deletion");
                    return false;
                }

                _unitOfWork.AppUserClubs.Delete(userClub);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"User club with ID {userClubId} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user club with ID: {userClubId}");
                throw;
            }
        }

        public async Task<IEnumerable<Club>> GetAllUserClubs()
        {
            try
            {
                _logger.LogInformation("Fetching all clubs for user");
                var clubs = await _unitOfWork.AppUserClubs.GetAllClubs();
                _logger.LogInformation($"Retrieved {clubs.Count()} clubs for user");
                return clubs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all clubs for user");
                throw;
            }
        }
    }
}

