using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Days { get; set; }

        [Required]
        [MaxLength(255)]
        public string Time { get; set; }

        [ForeignKey("Subject")]
        public int SubjectId { get; set; }

        // Navigation properties
        public Subject Subject { get; set; } // Subject for this group
        public ICollection<Booking> Bookings { get; set; } // Bookings for this group
        public ICollection<Session> Sessions { get; set; } // Sessions for this group
    }
}
