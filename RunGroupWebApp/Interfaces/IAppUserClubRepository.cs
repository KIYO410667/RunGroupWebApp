using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IAppUserClubRepository
    {
        Task<List<Club>> GetAllClubs();
        Task<AppUserClub> GetByIdAsync(int clubId);
        Task<List<AppUser>> GetAllUsers(int clubId);

        void Add(AppUserClub userClub);
        void Delete(AppUserClub userClub);
    }
}
