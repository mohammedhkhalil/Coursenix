using Coursenix.Models;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class StudentSettingsVM
    {
        // --- Account Info ---
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string fullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be an 11-digit number.")]
        public string? phone { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Parent number must be an 11-digit number.")]
        public string? parentPhone { get; set; }

        public int? gradeLevel { get; set; }

        // --- Password section ---
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string CurrPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string? password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string? ConfirmPassword { get; set; }
    }
}