using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.Services;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IClubRepository _clubRepository;
        private readonly IAzureBlobService _azureBlobService;

        public ClubController(ApplicationDbContext context, IClubRepository clubRepository
            , IAzureBlobService azureBlobService)
        {
            _context = context;
            _clubRepository = clubRepository;
            _azureBlobService = azureBlobService;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel ClubVM)
        {
            if (!ModelState.IsValid)
            {
                return View(ClubVM);
            }

            using (var stream = ClubVM.Image.OpenReadStream())
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ClubVM.Image.FileName);
                var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);

                // Save the blobUrl to your database 
                Club club = new Club
                {
                    Title = ClubVM.Title,
                    Description = ClubVM.Description,
                    Image = blobUrl,
                    Address = new Address
                    {
                        Street = ClubVM.Address.Street,
                        City = ClubVM.Address.City
                    }
                };

                _clubRepository.Add(club);

                return RedirectToAction("Index", new { message = "Photo uploaded successfully!" });
            }
        }
    }
}
