using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.Services;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRaceRepository _raceRepository;
        private readonly IAzureBlobService _azureBlobService;

        public RaceController(ApplicationDbContext context, IRaceRepository raceRepository
            , IAzureBlobService azureBlobService)
        {
            _context = context;
            _raceRepository = raceRepository;
            _azureBlobService = azureBlobService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetById(id);
            return View(race);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel RaceVM)
        {
            if (!ModelState.IsValid)
            {
                return View(RaceVM);
            }
            using (var stream = RaceVM.Image.OpenReadStream())
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(RaceVM.Image.FileName);
                var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);

                // Save the blobUrl to your database 
                Race race = new Race
                {
                    Title = RaceVM.Title,
                    Description = RaceVM.Description,
                    Image = blobUrl,
                    Address = new Address
                    {
                        Street = RaceVM.Address.Street,
                        City = RaceVM.Address.City
                    }
                };

                _raceRepository.Add(race);

                return RedirectToAction("Index", new { message = "Photo uploaded successfully!" });
            }
        }
    }
}
