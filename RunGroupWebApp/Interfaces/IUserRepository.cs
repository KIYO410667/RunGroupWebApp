using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserViewModel>> GetAllUser();
        Task<UserViewModel> GetUserSummaryById(string id);
        Task<List<Club>> GetClubsByUserId(string userId);
        void Add(AppUser user);
        void Delete(AppUser user);
        void Update(AppUser user);
    }
}
