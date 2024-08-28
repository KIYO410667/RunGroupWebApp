using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class AppUserClubController : Controller
    {
        private readonly IAppUserClubRepository _appUserClubRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public AppUserClubController(IAppUserClubRepository appUserClubRepository, IHttpContextAccessor contextAccessor)
        {
            _appUserClubRepository = appUserClubRepository;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userclub = await _appUserClubRepository.GetByIdAsync(id);
            if (userclub == null)
            {
                var userClub = new AppUserClub { AppUserId = userId, ClubId = id };
                _appUserClubRepository.Add(userClub);
                return RedirectToAction("Index", "Club");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userClub = await _appUserClubRepository.GetByIdAsync(id);
            if(userClub == null) { return View("Error"); }
            _appUserClubRepository.Delete(userClub);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
        {
            var clubs = await _appUserClubRepository.GetAllClubs();
            return View(clubs);
        }

       
    }
}
