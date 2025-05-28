using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key] public int Id { get; set; }

        // Parent grade-level 
        [ForeignKey(nameof(GradeLevel))] public int GradeLevelId { get; set; }

        // Schedule & logistics
        [StringLength(100)] public string? Name { get; set; }   // Optional 
        public List<string> SelectedDays { get; set; } = new List<string>();
        [Required] public TimeSpan StartTime { get; set; }
        [Required] public TimeSpan EndTime { get; set; }
        [Required, Range(1, int.MaxValue)] public int TotalSeats { get; set; }
        public int EnrolledStudentsCount { get; set; }
        [StringLength(200)] public string? Location { get; set; }

        // Navigation 
        public GradeLevel GradeLevel { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
