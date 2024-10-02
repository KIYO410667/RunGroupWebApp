using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Club>> GetAllUserClub();
        Task<AppUser> GetUserById();

        bool Update(AppUser user);
        bool Save();
    }
}
