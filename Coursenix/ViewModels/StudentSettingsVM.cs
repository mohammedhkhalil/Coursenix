using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class StudentSettingsVM
    {
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Phone]
        [Display(Name = "Parent's Phone Number")]
        public string ParentPhone { get; set; }

        [Required]
        [Display(Name = "Grade Level")]
        public int Grade { get; set; }

        // --- Password section ---
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Repeat Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
