using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Club>> GetAllUserClub();
        Task<List<Race>> GetAllUserRace();
        Task<AppUser> GetUserById(string id);

        bool Update(AppUser user);
        bool Save();
    }
}
