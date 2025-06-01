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
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string? password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string? ConfirmPassword { get; set; }
    }

}