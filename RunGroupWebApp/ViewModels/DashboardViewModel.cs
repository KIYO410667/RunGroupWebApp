using RunGroupWebApp.Models;

namespace RunGroupWebApp.ViewModels
{
    public class DashboardViewModel
    {
        public AppUser? appUser {  get; set; }
        public List<Club>? Clubs { get; set; }
    }
}
