using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IClubRepository _clubRepository;

        public ClubController(ApplicationDbContext context, IClubRepository clubRepository)
        {
            _context = context;
            _clubRepository = clubRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetById(id);
            return View(club);
        }

    }
}
