// Models/ViewModels/TeacherSettingsViewModel.cs
using System.ComponentModel.DataAnnotations; // For validation attributes
using Microsoft.AspNetCore.Http;

namespace Coursenix.ViewModels
{
    public class TeacherSettingsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } // Not required now
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Biography { get; set; }

        public string ExistingProfilePicture { get; set; }
        public IFormFile ProfilePicture { get; set; }

        // Password change
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

}