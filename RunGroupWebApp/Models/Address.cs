using RunGroupWebApp.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace RunGroupWebApp.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public City City { get; set; }

        public AppUser? AppUser { get; set; }
        public Club? Club { get; set; }
    }
}
