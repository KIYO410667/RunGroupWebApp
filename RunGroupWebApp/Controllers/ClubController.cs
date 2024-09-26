﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using System.Security.Claims;

namespace RunGroupWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IAzureBlobService azureBlobService,
            IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _azureBlobService = azureBlobService;
            _httpContextAccessor = httpContextAccessor;
        }

        private const int PageSize = 9; // 3x3 grid
        public async Task<IActionResult> Index(string keyword, ClubCategory? category, City? city, int page = 1)
        {
            // Store search parameters in ViewData for use in pagination links and form repopulation
            ViewData["Keyword"] = keyword;
            ViewData["Category"] = category?.ToString();
            ViewData["City"] = city?.ToString();

            var clubs = await _clubRepository.SearchClubsAsync(keyword, category, city, page, PageSize);
            var totalCount = await _clubRepository.GetSearchResultsCountAsync(keyword, category, city);

            var model = new PaginatedList<ClubSummaryViewModel>(clubs, totalCount, page, PageSize);

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetClubWithAppUserById(id);
            if(club == null) return View("Error");
            List<AppUser> users = await _clubRepository.GetAllUsers(id);
            ClubWithUsersViewModel clubUsers = new ClubWithUsersViewModel()
            {
                Club = club,
                AppUsers = users
            };
            return View(clubUsers);
        }

        public IActionResult Create()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clubVM = new CreateClubViewModel { AppUserId = userId };
            return View(clubVM);
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
                    AppUserId = ClubVM.AppUserId,
                    Address = new Address
                    {
                        Street = ClubVM.Address.Street,
                        City = ClubVM.Address.City
                    },
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
            
            // 4. Update the model
            _clubRepository.Update(userClub);

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(int id)
        {
            var club = await _clubRepository.GetById(id);
            if (club == null)
            {
                return View("Error");
            }
            return View(club);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _clubRepository.GetByIdIncludeAppUserClub(id);
            if (club == null)
            {
                return View("Error");
            }

            // Delete the associated image from Azure Blob Storage
            if (!string.IsNullOrEmpty(club.Image))
            {
                try
                {
                    await _azureBlobService.DeletePhotoByUrlAsync(club.Image);
                }
                catch (Exception ex)
                {
                    // Log the exception, but continue with the deletion process
                    // The photo might not exist, or there might be other issues
                    ModelState.AddModelError(string.Empty, "圖片刪除失敗，但會繼續刪除俱樂部資料");
                }
            }

            try
            {
                // Delete the club from the database
                _clubRepository.Delete(club);


                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Log the exception
                //_logger.LogError(ex, "Error deleting club {ClubId}", id);
                ModelState.AddModelError(string.Empty, "刪除俱樂部時發生錯誤。可能是因為此俱樂部仍有關聯的數據。");
                return View(club);
            }
        }
    }
}
