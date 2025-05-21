using System.ComponentModel.DataAnnotations;
using Coursenix.Models;

namespace Coursenix.ViewModels
{
    public class RegisterUserViewModel 
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be between {2} and {1} characters long.", MinimumLength = 6)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role Type is required.")]
        public string RoleType { get; set; } // "Student", "Teacher"

    
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be a 10-digit number.")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Parent number must be a 10-digit number.")]
        public string? ParentNumber { get; set; }  // for student

        public GradeLevel? Grade { get; set; } // Grade is nullable, and will be required in the Controller, based on RoleType

        // for teacher
        [StringLength(255, ErrorMessage = "Qualifications cannot be longer than 255 characters.")]
        public string? Biography { get; set; }

        [Required(ErrorMessage = "You must accept the terms and conditions.")]
        [Display(Name = "Terms and Conditions")]
        public bool Terms { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RoleType == "Student")
            {
                if (string.IsNullOrEmpty(ParentNumber))
                    yield return new ValidationResult("Parent number is required for students.", new[] { nameof(ParentNumber) });
                if (!Grade.HasValue)
                    yield return new ValidationResult("Grade is required for students.", new[] { nameof(Grade) });
            }
            else if (RoleType == "Teacher")
            {
                if (string.IsNullOrEmpty(Biography))
                    yield return new ValidationResult("Biography are required for teachers.", new[] { nameof(Biography) });
            }
        }
    }
}
