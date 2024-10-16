using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _unitOfWork.Users.GetAllUser();
                _logger.LogInformation($"Retrieved {users.Count()} users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users");
                throw; // Re-throw the exception to be handled by the controller
            }
        }

        public async Task<UserViewModel> GetUserSummaryById(string id)
        {
            try
            {
                _logger.LogInformation($"Fetching details for user with ID: {id}");
                var user = await _unitOfWork.Users.GetUserSummaryById(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found");
                    return null;
                }
                _logger.LogInformation($"Retrieved details for user {id}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching details for user {id}");
                throw; // Re-throw the exception to be handled by the controller
            }
        }
    }
}
