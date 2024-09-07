using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAll();
        Task<Club> GetById(int id);
        Task<Club> GetClubWithAppUserById(int id);
        //Task<IEnumerable<ClubSummaryViewModel>> SearchClubsAsync(string keyword, ClubCategory? category, City? city);
        Task<IEnumerable<ClubSummaryViewModel>> SearchClubsAsync(
        string keyword, ClubCategory? category, City? city,
        int page = 1, int pageSize = 9);
        Task<int> GetSearchResultsCountAsync(string keyword, ClubCategory? category, City? city);
        Task<IEnumerable<Club>> GetAllClubsByCity(string city);
        Task<List<Club>> GetClubsByUserId(string userId);
        bool Add(Club club);
        bool Update(Club club);
        bool Delete(Club club);
        bool Save();
    }
}
