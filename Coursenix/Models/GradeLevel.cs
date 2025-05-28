using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    [Table("GradeLevels")]
    public class GradeLevel
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Course))] public int CourseId { get; set; }

        // Business fields 
        [Required] public  int NumberOfGrade { get; set; } // 7 : 12 
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation 
        public Course Course { get; set; }
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public bool HasValue { get; internal set; }
    }
}
