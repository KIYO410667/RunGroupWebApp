using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces.IService
{
    public interface IClubService
    {
        Task<PaginatedList<ClubSummaryViewModel>> SearchClubs(string keyword, ClubCategory? category, City? city, int page, int pageSize);
        Task<ClubWithUsersViewModel> GetClubDetail(int id);
        Task<Club> GetClubById(int id);
        Task<bool> CreateClub(CreateClubViewModel clubVM);
        Task<EditClubViewModel> GetClubForEdit(int id);
        Task<bool> UpdateClub(int id, EditClubViewModel clubVM);
        Task<Club> GetClubForDelete(int id);
        Task<bool> DeleteClub(int id);
    }
}
