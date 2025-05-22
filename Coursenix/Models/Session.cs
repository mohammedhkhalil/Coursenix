using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models
{
    [Table("Sessions")]
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime SessionDateTime { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }

        // Navigation properties
        public Group Group { get; set; } // Group for this session
        public ICollection<Attendance> Attendances { get; set; } // Attendance records
    }
}
