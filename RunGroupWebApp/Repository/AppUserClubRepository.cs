using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Repository
{
    public class AppUserClubRepository : IAppUserClubRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppUserClubRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Add(AppUserClub userClub)
        {
            _context.AppUserClubs.Add(userClub);
            return Save();
        }

        public bool Delete(AppUserClub userClub)
        {
            _context.AppUserClubs.Remove(userClub);
            return Save();
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public async Task<List<Club>> GetAllClubs()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.AppUserClubs
                            .Where(auc => auc.AppUserId == userId)
                            .Select(auc => auc.Club)
                            .ToListAsync();
        }

        public async Task<AppUserClub> GetByIdAsync(int clubId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.AppUserClubs
                .Include(auc => auc.AppUser)
                .Include(auc => auc.Club)
                .FirstOrDefaultAsync(m => m.AppUserId == userId && m.ClubId == clubId);
        }

        //public Task<List<Club>> GetAllUsers()
        //{

        //}
    }
}
