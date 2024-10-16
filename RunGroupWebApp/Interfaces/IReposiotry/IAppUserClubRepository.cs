using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces.IReposiotry
{
    public interface IAppUserClubRepository : IGenericRepository<AppUserClub>
    {
        Task<List<Club>> GetAllClubs();
        Task<AppUserClub> GetByIdAsync(int clubId);
        Task<List<AppUser>> GetAllUsers(int clubId);
    }
}
