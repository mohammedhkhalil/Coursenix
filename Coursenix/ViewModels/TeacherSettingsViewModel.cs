// Models/ViewModels/TeacherSettingsViewModel.cs
using System.ComponentModel.DataAnnotations; // For validation attributes
using Microsoft.AspNetCore.Http;

namespace Coursenix.ViewModels
{
    public class TeacherSettingsViewModel
    {
        public int Id { get; set; }

        public string fullName { get; set; } // Not required now
        public string phone { get; set; }

        [EmailAddress]
        public string email { get; set; }

        [MaxLength(255)]
        public string bio { get; set; }
        public IFormFile ProfilePicture { get; set; }

        // Password change
        [Required]
        public string CurrentPassword { get; set; }
        public string? password { get; set; }

        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }

}