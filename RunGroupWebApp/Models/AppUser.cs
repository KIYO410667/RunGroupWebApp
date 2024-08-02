using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RunGroupWebApp.Models
{
    public class AppUser : IdentityUser
    {
        [Key]
        public string Id { get; set; }
        public Address? Address { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
    }
}
