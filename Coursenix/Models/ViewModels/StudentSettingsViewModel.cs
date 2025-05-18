using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models.ViewModels
{
    public class StudentSettingsViewModel
    {
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string Name { get; set; }
        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [MaxLength(11, ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(@"(010|011|012|015)\d{8}", ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [MaxLength(11, ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(@"(010|011|012|015)\d{8}", ErrorMessage = "Enter a valid phone number")]
        public string ParentNumber { get; set; }
        public GradeLevel Grade { get; set; }

        public string? CurrentPassword { get; set; }

        [MinLength(8)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}