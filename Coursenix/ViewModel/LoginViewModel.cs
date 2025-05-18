using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModel
{
    public class LoginViewModel
    {

        [Required]
        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [MinLength(8, ErrorMessage = "Use 8 characters or more for your password\r\n")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Student" or "Teacher"
    }
}
