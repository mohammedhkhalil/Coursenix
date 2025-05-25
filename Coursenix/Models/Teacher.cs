using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; } // Foreign key to AppUser

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; } // Navigation property

        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        [MaxLength(255)]
        public string? Biography { get; set; }

        [MaxLength(255)]
        public string? ProfilePicture { get; set; }

        // Navigation properties
        public ICollection<Subject> Subjects { get; set; }
        public ICollection<Group> Groups { get; set; }
    }
}