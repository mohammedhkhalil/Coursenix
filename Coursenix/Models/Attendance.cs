using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Session")]
        public int SessionId { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        // Navigation properties
        public Student Student { get; set; } // Student attendance record
        public Session Session { get; set; } // Session attended
    }
}
