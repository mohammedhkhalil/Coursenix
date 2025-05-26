// Models/ViewModels/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}