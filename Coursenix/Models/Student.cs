using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
   
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string AppUserId { get; set; } // Foreign key to AppUser
        public AppUser AppUser { get; set; } // Navigation property

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        public GradeLevel? Grade { get; set; }
        public string PhoneNumber { get; set; }
        public string? ParentPhoneNumber { get; set; }

        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
    }
}