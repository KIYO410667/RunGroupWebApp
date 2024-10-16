using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces.IReposiotry;
using RunGroupWebApp.Models;
using System.Security.Claims;

namespace RunGroupWebApp.Repository
{
    public class AppUserClubRepository : GenericRepository<AppUserClub>, IAppUserClubRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppUserClubRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Club>> GetAllClubs()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.AppUserClubs.Include(auc => auc.Club)
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

        public async Task<List<AppUser>> GetAllUsers(int clubId)
        {
            return await _context.AppUserClubs.Include(auc => auc.AppUser)
                                .Where(c => c.ClubId == clubId)
                                .Select(auc => auc.AppUser)
                                .ToListAsync();
        }
    }
}
