using RunGroupWebApp.Models;
using RunGroupWebApp.Repository;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces.IReposiotry
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<List<UserViewModel>> GetAllUser();
        Task<UserViewModel> GetUserSummaryById(string id);
        Task<List<Club>> GetClubsByUserId(string userId);
    }
}
