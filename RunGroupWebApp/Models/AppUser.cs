using Microsoft.AspNetCore.Identity;

namespace RunGroupWebApp.Models
{
    public class AppUser : IdentityUser
    {
        public Address? Address { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
    }
}
