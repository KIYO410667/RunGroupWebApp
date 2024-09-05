using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;

        public ClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Club club)
        {
            _context.Clubs.Add(club);
            return Save();
        }

        public bool Delete(Club club)
        {
            _context.Clubs.Remove(club);
            return Save();
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

        public async Task<Club> GetClubWithAppUserById(int id)
        {
            return await _context.Clubs.Include(a => a.Address).Include(a => a.AppUser).FirstOrDefaultAsync(i => i.Id == id);
        }
        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool Update(Club club)
        {
            _context.Update(club);
            return Save();
        }

        public async Task<List<Club>> GetClubsByUserId(string userId)
        {
            return await _context.Clubs.Include(au => au.AppUser).Where(au => au.AppUserId == userId).ToListAsync();
        }
    }
}
