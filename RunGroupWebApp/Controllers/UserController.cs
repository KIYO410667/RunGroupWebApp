using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var results = await _userRepository.GetAllUser();
                _logger.LogInformation($"Retrieved {results.Count()} users");
                return View(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users");
                return View("Error");
            }
        }

        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                _logger.LogInformation($"Fetching details for user with ID: {id}");
                var result = await _userRepository.GetUserSummaryById(id);
                if (result == null)
                {
                    _logger.LogWarning($"User with ID {id} not found");
                    return View("Error");
                }
                _logger.LogInformation($"Returning detail view for user {id}");
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching details for user {id}");
                return View("Error");
            }
        }
    }
}
