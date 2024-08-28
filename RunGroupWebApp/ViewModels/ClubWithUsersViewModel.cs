using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RunGroupWebApp.ViewModels
{
    public class ClubWithUsersViewModel
    {
        public Club Club { get; set; }
        public List<AppUser>? AppUsers { get; set; }
    }
}
