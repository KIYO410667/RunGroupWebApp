﻿using RunGroupWebApp.Data.Enum;
using System.ComponentModel.DataAnnotations;
using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.ViewModels
{
    public class EditUserProfileViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public IFormFile? ProfilePhotoFile { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public Address? Address { get; set; }
    }
}
