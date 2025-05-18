using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace Coursenix.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
        public string Password { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string Name { get; set; }

        [MaxLength(11,ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(@"(010|011|012|015)\d{8}", ErrorMessage = "Enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [MaxLength(255)]
        public string ProfilePicture { get; set; }

        public string? Qualifications { get; set; }

        // Navigation properties
        public ICollection<Subject> Subjects { get; set; } // Subjects taught
        public ICollection<Group> Groups { get; set; } // Groups managed

    }
}
