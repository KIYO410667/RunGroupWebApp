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

            // AppUser and Club many-to-many relationship
            modelBuilder.Entity<AppUserClub>(entity =>
            {
                entity.HasKey(uc => new { uc.AppUserId, uc.ClubId });

                entity.HasOne(uc => uc.AppUser)
                    .WithMany(u => u.AppUserClubs)
                    .HasForeignKey(uc => uc.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(uc => uc.Club)
                    .WithMany(c => c.AppUserClubs)
                    .HasForeignKey(uc => uc.ClubId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(uc => new { uc.AppUserId, uc.ClubId })
                        .HasDatabaseName("IX_ParticipantIdClubId");
            });

            // AppUser and Club one-to-many relationship (Club creator)
            modelBuilder.Entity<Club>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.AppUser)
                    .WithMany(u => u.CreatedClubs)
                    .HasForeignKey(c => c.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Address)
                        .WithOne(a => a.Club)
                        .HasForeignKey<Club>(c => c.AddressId)
                        .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.Id).IsUnique().HasDatabaseName("IX_ClubId");
                entity.HasIndex(c => c.AppUserId).HasDatabaseName("IX_CreatedClubAppUserId");
            });

            // Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.Id).HasDatabaseName("IX_AddressId");
            });

            // AppUser
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasOne(u => u.Address)
                    .WithOne(a => a.AppUser)
                    .HasForeignKey<AppUser>(u => u.AddressId) 
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(u => u.Id).IsUnique().HasDatabaseName("IX_AppUserId");
            });
        }
    }
}
