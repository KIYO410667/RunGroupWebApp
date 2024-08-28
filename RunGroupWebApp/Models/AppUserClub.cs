namespace RunGroupWebApp.Models
{
    public class AppUserClub
    {
        public string AppUserId { get; set; }

        public AppUser AppUser { get; set; }

        public int ClubId { get; set; }

        public Club Club { get; set; }
    }
}
