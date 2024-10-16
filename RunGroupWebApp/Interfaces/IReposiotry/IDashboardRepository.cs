using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;

namespace RunGroupWebApp.Interfaces.IReposiotry
{
    public interface IDashboardRepository
    {
        Task<List<Club>> GetAllUserClub();
        Task<AppUser> GetUserById();
    }
}
