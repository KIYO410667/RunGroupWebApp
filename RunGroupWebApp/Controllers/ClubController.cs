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

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetById(id);
            if(club == null) return View("Error");
            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                URL = club.Image,
                AddressId = club.AddressId,
                Address = club.Address,
                ClubCategory = club.ClubCategory,
            };
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "輸入失敗，請重新嘗試");
                return View(clubVM);
            }

            // 1. Find the userClub by Id
            var userClub = await _clubRepository.GetById(id);

            if (userClub == null)
            {
                return View("Error");
            }

            // 2. Try to delete the existing photo, considering the case where it might not exist
            if (!string.IsNullOrEmpty(userClub.Image) && clubVM.Image != null)
            {
                try
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    // Log the exception, but continue with the update process
                    // The photo might not exist, or there might be other issues
                    ModelState.AddModelError(string.Empty, "圖片刪除失敗");
                    return View(clubVM);
                }
            }

            // 3. Update the userClub properties directly
            userClub.Title = clubVM.Title;
            userClub.Description = clubVM.Description;
            userClub.AddressId = clubVM.AddressId;
            userClub.Address = clubVM.Address;
            userClub.ClubCategory = clubVM.ClubCategory;

            // Handle the new image upload
            if (clubVM.Image != null)
            {
                using (var stream = clubVM.Image.OpenReadStream())
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(clubVM.Image.FileName);
                    var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                    if (blobUrl == null)
                    {
                        ModelState.AddModelError("Image", "Photo upload failed");
                        return View(clubVM);
                    }
                    userClub.Image = blobUrl; //only change the Image URL
                }
            }
            else
            {

            }
            // 4. Update the model
            _clubRepository.Update(userClub);

            return RedirectToAction("Index");
        }
    }
}
