using Coursenix.Models;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class StudentSettingsVM
    {
        // --- Account Info ---
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be an 11-digit number.")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Parent number must be an 11-digit number.")]
        public string? ParentPhoneNumber { get; set; }

        public int? gradeLevel { get; set; }

        // --- Password section ---
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string? CurrPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}