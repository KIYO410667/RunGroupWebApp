using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces.IReposiotry;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<List<Club>> GetAllUserClub()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Clubs.Where(a => a.AppUser.Id == userId).ToListAsync();
        }

        public async Task<AppUser> GetUserById()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Users.Include(a => a.Address).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
