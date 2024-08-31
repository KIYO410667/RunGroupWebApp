using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;
        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Race race)
        {
            _context.Races.Add(race);
            return Save();
        }

        public bool Delete(Race race)
        {
            _context.Races.Remove(race);
            return Save();

        }

        public async Task<IEnumerable<Race>> GetAll()
        {
            return await _context.Races.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
        {
            /*return await _context.Races.Include(a => a.Address).Where(c => c.Address.City.Contains(city)).ToListAsync();*/
            return await _context.Races
                         .Include(a => a.Address)
                         .Where(c => Enum.GetValues(typeof(City))
                                         .Cast<City>()
                                         .Any(e => e.ToString().Contains(city) && e == c.Address.City))
                         .ToListAsync();
        }

        public async Task<Race> GetById(int id)
        {
            return await _context.Races.Include(a => a.Address).FirstOrDefaultAsync(i => i.Id == id);
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0 ? true : false;
        }

        public bool Update(Race race)
        {
            _context.Races.Update(race);
            return Save();
        }
    }
}
