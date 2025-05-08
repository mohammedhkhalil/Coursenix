// Models/ViewModels/TeacherSignUpViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models.ViewModels
{
    public class TeacherSignUpViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } // Or separate into FirstName/LastName if needed

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Create a password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Terms is Required")]
        [Display(Name = "Terms Agreement")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and privacy policy")]
        public bool TermsAccepted { get; set; }
    }
}