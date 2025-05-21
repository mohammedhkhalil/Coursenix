// Models/ViewModels/TeacherSettingsViewModel.cs
using System.ComponentModel.DataAnnotations; // For validation attributes
using Microsoft.AspNetCore.Http;

namespace Coursenix.ViewModels
{
    public class TeacherSettingsViewModel
    {
        public int TeacherId { get; set; } // Or string if using IdentityUser.Id

        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        [MaxLength(100)] // Adjust max length as per your Teacher model or requirements
        public string FullName { get; set; } // Combining First and Last name for display/input

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        [Phone] // Basic phone number format validation
        [MaxLength(20)] // Match Teacher model
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [Display(Name = "Email Address")]
        [EmailAddress] // Basic email format validation
        [MaxLength(255)] // Match Teacher model
        public string Email { get; set; }

        [Display(Name = "Biography")]
        [MaxLength(1000)] // Match Teacher model
        public string Biography { get; set; }

        public string CurrentProfilePictureUrl { get; set; }

        // Property for uploading a new profile picture
        [Display(Name = "Profile Photo")]
        // Optional: Add custom validation attributes for file size/type
        // [MaxFileSize(1 * 1024 * 1024)] // Example: Max 1MB (Requires implementing custom attribute)
        // [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })] // Example: Allowed extensions (Requires implementing custom attribute)
        public IFormFile NewProfilePictureFile { get; set; } // Represents the uploaded file


        // --- Password Change Fields ---
        // These should only be validated/processed if the user intends to change the password

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        // [Required] // Don't make Required unless the user enters anything in NewPassword/ConfirmPassword
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        // [Required] // Don't make Required unless the user enters something here
        // Add password complexity requirements if needed (e.g., [RegularExpression], [StringLength])
        [StringLength(100, MinimumLength = 8, ErrorMessage = "New Password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")] // Compare with NewPassword
        // [Required] // Don't make Required unless the user enters anything here
        public string ConfirmNewPassword { get; set; }

        // Property to hold messages (success or error)
        public string StatusMessage { get; set; }
        public bool IsSuccess { get; set; } = false; // To differentiate success from error messages

    }
}