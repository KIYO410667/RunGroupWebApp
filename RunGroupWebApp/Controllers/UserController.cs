using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces.IReposiotry;
using RunGroupWebApp.Interfaces.IService;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var results = await _userService.GetAllUsers();
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
                var result = await _userService.GetUserSummaryById(id);
                if (result == null)
                {
                    _logger.LogWarning($"User with ID {id} not found");
                    return View("Error");
                }
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
