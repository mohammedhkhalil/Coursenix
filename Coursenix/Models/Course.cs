using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key] public int Id { get; set; }

        // Basic info 
        [Required, MaxLength(255)] public string Name { get; set; }
        [MaxLength(1000)] public string? Description { get; set; }
        [MaxLength(255)] public string? Location { get; set; }
        [MaxLength(255)] public string? ThumbnailFileName { get; set; }
        public int StudentsPerGroup { get; set; }

        // Relations
        [ForeignKey(nameof(Teacher))] public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<GradeLevel> GradeLevels { get; set; } = new List<GradeLevel>();
    }
}
