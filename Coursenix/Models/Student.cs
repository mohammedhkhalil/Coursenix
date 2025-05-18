using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{
    public enum GradeLevel
    {
        Grade7 = 7,
        Grade8 = 8,
        Grade9 = 9,
        Grade10 = 10,
        Grade11 = 11,
        Grade12 = 12
    }

    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }


        [MaxLength(11, ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(@"(010|011|012|015)\d{8}", ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select your grade")]
        public GradeLevel Grade { get; set; }

        [MaxLength(11, ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(@"(010|011|012|015)\d{8}", ErrorMessage = "Enter a valid phone number")]
        public string ParentNumber { get; set; } // Added Parent Number property

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(255)]
        [MinLength(8,ErrorMessage = "Use 8 characters or more for your password\r\n")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password does not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; } // Bookings made by this student
        public ICollection<Attendance>? Attendances { get; set; } // Attendance records
    }
}
