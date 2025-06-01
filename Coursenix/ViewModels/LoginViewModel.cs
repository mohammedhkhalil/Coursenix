using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class LoginViewModel
    {




        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Student or Teacher or Admin

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
