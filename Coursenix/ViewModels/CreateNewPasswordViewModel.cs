using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class CreateNewPasswordViewModel
    {

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Use 8 characters or more for your password\r\n", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}
