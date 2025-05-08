using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Coursenix.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SubjectName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public int GradeLevel { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public decimal Price { get; set; }

        [MaxLength(255)] // أو طول مناسب لمسار الملف that added by Abdalrhman
        public string ThumbnailFileName { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        // Navigation properties
        public Teacher Teacher { get; set; } // Teacher teaching this subject
        public ICollection<Group> Groups { get; set; } // Groups for this subject
        public ICollection<Booking> Bookings { get; set; } // Bookings for this subject
    }
}
