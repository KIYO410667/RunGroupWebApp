using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
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
    }
}
