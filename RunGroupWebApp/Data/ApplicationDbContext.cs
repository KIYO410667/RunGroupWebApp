using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUserClub> AppUserClubs { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppUser 和 Club 之間的多對多關係
            modelBuilder.Entity<AppUserClub>()
                .HasKey(uc => new { uc.AppUserId, uc.ClubId });

            modelBuilder.Entity<AppUserClub>()
                .HasOne(uc => uc.AppUser)
                .WithMany(u => u.AppUserClubs)
                .HasForeignKey(uc => uc.AppUserId);

            modelBuilder.Entity<AppUserClub>()
                .HasOne(uc => uc.Club)
                .WithMany(c => c.AppUserClubs)
                .HasForeignKey(uc => uc.ClubId);

            // AppUser 和 Club 之間的一對多關係 (一個 Club 只能由一個 AppUser 創建)
            modelBuilder.Entity<Club>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.CreatedClubs)
                .HasForeignKey(c => c.AppUserId);
        }
    }
}
