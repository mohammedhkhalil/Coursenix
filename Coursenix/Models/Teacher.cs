using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key] public int Id { get; set; }

        // Identity linking 
        public string AppUserId { get; set; } // Foreign key to AppUser
        public AppUser AppUser { get; set; } // Navigation property

        // Profile 
        [Required, MaxLength(255)] public string Name { get; set; }
        [Required] public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [MaxLength(255)] public string? Biography { get; set; }
        [MaxLength(255)] public string? ProfilePicture { get; set; }

        // Navigation 
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
