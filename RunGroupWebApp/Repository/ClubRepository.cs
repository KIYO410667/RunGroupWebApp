using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;

        public ClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Club club)
        {
            _context.Clubs.Add(club);
        }
        public void Delete(Club club)
        {
            _context.Clubs.Remove(club);
        }
        public void Update(Club club)
        {
            _context.Update(club);
        }
        public async Task<IEnumerable<Club>> GetAll()
        {
            return await _context.Clubs.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<IEnumerable<Club>> GetAllClubsByCity(string city)
        {
            //return await _context.Clubs.Include(a => a.Address).Where(c => c.Address.City.Contains(city)).ToListAsync();
            return await _context.Clubs
                         .Include(a => a.Address)
                         .Where(c => Enum.GetValues(typeof(City))
                                         .Cast<City>()
                                         .Any(e => e.ToString().Contains(city) && e == c.Address.City))
                         .ToListAsync();
        }

        public async Task<Club> GetById(int id)
        {
            return await _context.Clubs.Include(a => a.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Club> GetByIdIncludeAppUserClub(int id)
        {
            return await _context.Clubs.Include(ac => ac.AppUserClubs).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Club> GetClubWithAppUserById(int id)
        {
            return await _context.Clubs.Include(a => a.Address).Include(a => a.AppUser).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<AppUser>> GetAllUsers(int clubId)
        {
            return await _context.AppUserClubs.Include(auc => auc.AppUser)
                                .Where(c => c.ClubId == clubId)
                                .Select(auc => auc.AppUser)
                                .ToListAsync();
        }

        public async Task<List<Club>> GetClubsByUserId(string userId)
        {
            return await _context.Clubs.Where(au => au.AppUserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<ClubSummaryViewModel>> SearchClubsAsync(string keyword, ClubCategory? category, City? city, int page = 1, int pageSize = 9)
        {
            IQueryable<Club> query = _context.Clubs;

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c =>
                    EF.Functions.Like(c.Title.ToLower(), $"%{keyword}%") ||
                    EF.Functions.Like(c.Description.ToLower(), $"%{keyword}%"));
            }

            if (category.HasValue)
            {
                query = query.Where(c => c.ClubCategory == category.Value);
            }

            if (city.HasValue)
            {
                query = query.Where(c => c.Address.City == city.Value);
            }

            // Add OrderBy before Skip and Take
            query = query.OrderBy(c => c.Id);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClubSummaryViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.Image,
                    UserName = c.AppUser.UserName,
                    ProfilePhotoUrl = c.AppUser.ProfilePhotoUrl
                })
                .ToListAsync();
        }

        public async Task<int> GetSearchResultsCountAsync(string keyword, ClubCategory? category, City? city)
        {
            IQueryable<Club> query = _context.Clubs;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));
            }

            if (category.HasValue)
            {
                query = query.Where(c => c.ClubCategory == category.Value);
            }

            if (city.HasValue)
            {
                query = query.Where(c => c.Address.City == city.Value);
            }

            return await query.CountAsync();
        }

    }
}
