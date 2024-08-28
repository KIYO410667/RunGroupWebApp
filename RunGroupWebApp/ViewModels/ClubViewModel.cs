using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RunGroupWebApp.ViewModels
{
    public class ClubViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public ClubCategory ClubCategory { get; set; }

        public int AddressId { get; set; }
        public Address? Address { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public List<AppUserClub> AppUserClubs { get; set; }
    }
}
