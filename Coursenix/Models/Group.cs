using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Coursenix.Models
{
    [Table("Groups")]
    public class Group
    {


        [Key]
        public int GroupId { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }


        [Required]
        [MaxLength(50)] // "Tuesday & Thursday"
        public string DayOfWeek { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int EnrolledStudentsCount { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }
        // Navigation properties
        public Subject Subject { get; set; } // Subject for this group
        public ICollection<Booking> Bookings { get; set; } // Bookings for this group                                          
        public ICollection<Session> Sessions { get; set; } // Sessions for this group
        public ICollection<Session> Student { get; set; } // Sessions for this group
    }
}
