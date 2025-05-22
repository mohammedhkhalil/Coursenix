// Models/Group.cs - TeacherId REMOVED to eliminate FK_Groups_Teachers_TeacherId constraint

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]

namespace Coursenix.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }



        [Required]
        [Range(7, 12, ErrorMessage = "Grade must be between 7 and 12.")]
        public int Grade { get; set; }

        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Name { get; set; } // Group name can be optional if not [Required]

        [Required(ErrorMessage = "Days of the week are required.")]
        [StringLength(50, ErrorMessage = "Days string cannot exceed 50 characters.")]
        public string DayOfWeek { get; set; } // Storing days as a string (e.g., "Mon, Wed")

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.DateTime)] // Using DateTime to store both date and time, EF Core handles time part
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        // Total number of seats available in this group
        [Required(ErrorMessage = "Total seats are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1.")]
        public int TotalSeats { get; set; }


        public int EnrolledStudentsCount { get; set; } 

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        public ICollection<GroupDay> GroupDays { get; set; } = new List<GroupDay>(); // days related with group
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}