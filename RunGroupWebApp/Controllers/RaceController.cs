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

        public async Task<IActionResult> Edit(int id)
        {
            var userRace = await _raceRepository.GetById(id);
            if(userRace == null) {return View("Error");}
            EditRaceViewModel raceVM = new EditRaceViewModel
            {
                Address = userRace.Address,
                Title = userRace.Title,
                Description = userRace.Description,
                URL = userRace.Image,
                AddressId = userRace.AddressId,
                RaceCategory = userRace.RaceCategory
            };
            return View(raceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "輸入失敗，請重試");
                return View("Error");
            }
            // 1. Find the userRace by Id
            var userRace = await _raceRepository.GetById(id);
            if(userRace == null) {
                ModelState.AddModelError("", "使用者不存在，請重試");
                return View("Error");
            }

            userRace.Id = raceVM.Id;
            userRace.Title = raceVM.Title;
            userRace.Description = raceVM.Description;
            userRace.Address = raceVM.Address;
            userRace.AddressId = raceVM.AddressId;
            userRace.RaceCategory = raceVM.RaceCategory;

            //2. Try to delete the existing photo, considering the case where it might not exist and user might not upload new photo
            if (raceVM.Image != null)
            {
                if (!string.IsNullOrEmpty(userRace.Image))
                {
                    try
                    {
                        await _azureBlobService.DeletePhotoByUrlAsync(userRace.Image);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception, but continue with the update process
                        // The photo might not exist, or there might be other issues
                        ModelState.AddModelError(string.Empty, "圖片刪除失敗");
                        return View(raceVM);
                    }
                }
                //3. Upload the photo to AzureBlob if there's a file being uploading
                using (var stream = raceVM.Image.OpenReadStream())
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(raceVM.Image.FileName);
                    var blobUrl = await _azureBlobService.UploadPhotoAsync(stream, fileName);
                    if (blobUrl == null)
                    {
                        ModelState.AddModelError("Image", "Photo upload failed");
                        return View(raceVM);
                    }
                    userRace.Image = blobUrl; //only change the Image URL
                }

            }
            
            _raceRepository.Update(userRace);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int id)
        {
            var race = await _raceRepository.GetById(id);
            if (race == null)
            {
                return View("Error");
            }
            return View(race);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _raceRepository.GetById(id);
            if (race == null)
            {
                return View("Error");
            }

            // Delete the associated image from Azure Blob Storage
            if (!string.IsNullOrEmpty(race.Image))
            {
                try
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(race.Image);
                }
                catch (Exception ex)
                {
                    // Log the exception, but continue with the deletion process
                    // The photo might not exist, or there might be other issues
                    ModelState.AddModelError(string.Empty, "圖片刪除失敗，但會繼續刪除俱樂部資料");
                }
            }

            // Delete the club from the database
            _raceRepository.Delete(race);

            return RedirectToAction("Index");
        }
    }
}
