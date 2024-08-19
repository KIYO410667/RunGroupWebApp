using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGroupWebApp.Models
{
    public class AppUser : IdentityUser
    {

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public Address? Address { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }

        //public ICollection<Club> OrganizedEvents { get; set; }
        //public ICollection<Club> ParticipatingEvents { get; set; }
    }
}
