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

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(255)]
        public string ProfilePicture { get; set; }

        [MaxLength(255)]
        public string Qualifications { get; set; }

        // Navigation properties
        public ICollection<Subject> Subjects { get; set; } // Subjects taught
        public ICollection<Group> Groups { get; set; } // Groups managed
    }
}
