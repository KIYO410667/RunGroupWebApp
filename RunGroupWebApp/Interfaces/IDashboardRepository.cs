using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Club>> GetAllUserClub();
        Task<AppUser> GetUserById();

        void Update(AppUser user);
    }
}
