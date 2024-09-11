using RunGroupWebApp.Data.Enum;

namespace RunGroupWebApp.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public City? city { get; set; }
        public int ClubNumber { get; set; } = 0;
    }
}
