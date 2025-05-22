using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; } // Foreign key to AppUser

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; } // Navigation property
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        public GradeLevel Grade { get; set; }
        public string PhoneNumber { get; set; }
        public string ParentPhoneNumber { get; set; }

        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
    }
}