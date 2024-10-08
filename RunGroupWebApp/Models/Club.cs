﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RunGroupWebApp.Data.Enum;

namespace RunGroupWebApp.Models
{
    public class Club
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public ClubCategory ClubCategory { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        [Range(0, 500, ErrorMessage = "ParticipantsCount cannot be negative")]
        public int ParticipantsCount { get; set; } = 0;

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public ICollection<AppUserClub> AppUserClubs { get; set; }
    }
}
