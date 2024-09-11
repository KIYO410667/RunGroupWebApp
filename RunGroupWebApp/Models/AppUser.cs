using Microsoft.AspNetCore.Identity;
using RunGroupWebApp.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGroupWebApp.Models
{
    public class AppUser : IdentityUser
    {

        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        //public int? Pace { get; set; }
        //public int? Mileage { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Bio { get; set; }

        public string? Street { get; set; }
        public City? City { get; set; }

        public ICollection<Club> CreatedClubs { get; set; }
        public ICollection<AppUserClub> AppUserClubs { get; set; }
    }
}
