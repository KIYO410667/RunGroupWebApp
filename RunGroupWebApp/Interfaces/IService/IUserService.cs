using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsers();
        Task<UserViewModel> GetUserSummaryById(string id);
    }
}
