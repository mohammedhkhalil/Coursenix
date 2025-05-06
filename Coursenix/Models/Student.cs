using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{

    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

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

        public int GradeLevel { get; set; }

        [MaxLength(20)]
        public string ParentNumber { get; set; } // Added Parent Number property

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } // Bookings made by this student
        public ICollection<Attendance> Attendances { get; set; } // Attendance records
    }
}
