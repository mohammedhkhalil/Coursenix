using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }

        //[Required]
        public DateTime BookingDate { get; set; }

        // Navigation properties
        public Student Student { get; set; } // Student making this booking
        public Group Group { get; set; } // Group being booked
    }
}
