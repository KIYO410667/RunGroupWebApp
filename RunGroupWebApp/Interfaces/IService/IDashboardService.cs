using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Interfaces.IService
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardInformation();
        Task<EditUserProfileViewModel> GetUserProfileForEdit();
        Task<bool> UpdateUserProfile(EditUserProfileViewModel editUserVM);
    }
}
