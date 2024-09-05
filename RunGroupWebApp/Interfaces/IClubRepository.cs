using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAll();
        Task<Club> GetById(int id);
        Task<Club> GetClubWithAppUserById(int id);
        Task<IEnumerable<Club>> GetAllClubsByCity(string city);
        Task<List<Club>> GetClubsByUserId(string userId);
        bool Add(Club club);
        bool Update(Club club);
        bool Delete(Club club);
        bool Save();
    }
}
