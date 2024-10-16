using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces.IService
{
    public interface IAppUserClubService
    {
        Task<bool> AddUserToClub(int clubId);
        Task<bool> RemoveUserFromClub(int userClubId);
        Task<IEnumerable<Club>> GetAllUserClubs();
    }
}
