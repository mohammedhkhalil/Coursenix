// Models/ViewModels/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}