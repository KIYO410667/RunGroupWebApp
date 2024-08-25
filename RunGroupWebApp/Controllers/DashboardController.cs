using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardVM = new DashboardViewModel()
            {
                Clubs = await _dashboardRepository.GetAllUserClub(),
                Races = await _dashboardRepository.GetAllUserRace()
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
                Pace = currentUser.Pace,
                Mileage = currentUser.Mileage,
                ProfilePhotoUrl = currentUser.ProfilePhotoUrl,

                City = currentUser.City,
                Street = currentUser.Street,
            };
            return View(editUserVM);
        }

    }
}
