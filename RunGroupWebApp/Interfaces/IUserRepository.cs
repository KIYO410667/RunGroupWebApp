using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserViewModel>> GetAllUser();
        Task<UserViewModel> GetUserSummaryById(string id);
        Task<List<Club>> GetClubsByUserId(string userId);
        bool Add(AppUser user);
        bool Delete(AppUser user);
        bool Update(AppUser user);
        bool Save();
    }
}
