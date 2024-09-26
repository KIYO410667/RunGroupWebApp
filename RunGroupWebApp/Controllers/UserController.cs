using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var results = await _userRepository.GetAllUser();
            List<UserViewModel> userVms = new List<UserViewModel>();
            foreach (AppUser user in results)
            {
                UserViewModel userVM = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Bio = user.Bio,
                    ProfilePhotoUrl = user.ProfilePhotoUrl,                };
                userVms.Add(userVM);
            }
            return View(userVms);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var result = await _userRepository.GetUserById(id);
            var clubs = await _userRepository.GetClubsByUserId(id);
            var userVM = new UserViewModel()
            {
                Id = result.Id,
                UserName = result.UserName,
                Bio = result.Bio,
                ProfilePhotoUrl = result.ProfilePhotoUrl,
                city = result.Address.City,
                ClubNumber = clubs.Count(),
                clubs = clubs
            };
            return View(userVM);
        }

    }
}
