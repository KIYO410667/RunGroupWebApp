using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public async Task<List<Club>> GetAllUserClub()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //// 確保 HttpContext 和 User 不為 null
            //var httpContext = _httpContextAccessor.HttpContext;
            //var user = httpContext?.User;

            //if (user == null)
            //{
            //    // 處理 User 為 null 的情況
            //    throw new UnauthorizedAccessException("User is not authenticated.");
            //}

            //var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            // 根據用戶ID過濾文章
            return await _context.Clubs
                                 .Where(a => a.Organizer.Id == user.ToString())
                                 .ToListAsync();
        }

        public async Task<List<Race>> GetAllUserRace()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 根據用戶ID過濾文章
            return await _context.Races
                                 .Where(a => a.AppUser.Id == user.ToString())
                                 .ToListAsync();
        }
    }
}
