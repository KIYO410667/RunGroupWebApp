using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces.IReposiotry
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<IEnumerable<Club>> GetAll();
        Task<Club> GetById(int id);
        Task<Club> GetClubWithAppUserById(int id);
        Task<Club> GetByIdIncludeAppUserClub(int id);
        Task<List<AppUser>> GetAllUsers(int clubId);

        Task<IEnumerable<ClubSummaryViewModel>> SearchClubsAsync(
        string keyword, ClubCategory? category, City? city,
        int page = 1, int pageSize = 9);
        Task<int> GetSearchResultsCountAsync(string keyword, ClubCategory? category, City? city);
        Task<IEnumerable<Club>> GetAllClubsByCity(string city);
        Task<List<Club>> GetClubsByUserId(string userId);
    }
}
