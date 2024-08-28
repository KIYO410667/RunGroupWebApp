using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IAppUserClubRepository
    {
        Task<List<Club>> GetAllClubs();
        Task<AppUserClub> GetByIdAsync(int clubId);
        //Task<List<Club>> GetAllUsers();

        bool Add(AppUserClub userClub);
        bool Save();
        bool Delete(AppUserClub userClub);
    }
}
