using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace RunGroupWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(AppUser user)
        {
            _context.Users.Add(user);
        }

        public void Delete(AppUser user)
        {
            _context.Users.Remove(user);
        }

        public async Task<List<UserViewModel>> GetAllUser()
        {
            return await _context.Users.
                Select(c => new UserViewModel()
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Bio = c.Bio,
                    ProfilePhotoUrl = c.ProfilePhotoUrl
                })
                .ToListAsync();
        }

        public async Task<UserViewModel> GetUserSummaryById(string id)
        {
            return await _context.Users
                .Select(c => new UserViewModel()
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Bio = c.Bio,
                    ProfilePhotoUrl = c.ProfilePhotoUrl,
                    city = c.Address.City,
                    ClubNumber = c.CreatedClubs.Count(),
                    clubs = c.CreatedClubs.ToList()
                }).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Club>> GetClubsByUserId(string userId)
        {
            return await _context.Clubs.Where(au => au.AppUserId == userId).ToListAsync();
        }

        public void Update(AppUser user)
        {
            _context.Users.Update(user);
        }
    }
}
