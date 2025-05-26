using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class LoginViewModel
    {


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [MinLength(6, ErrorMessage = "Use 6 characters or more for your password\r\n")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Student or Teacher or Admin

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
