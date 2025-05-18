// Models/Group.cs - TeacherId REMOVED to eliminate FK_Groups_Teachers_TeacherId constraint

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]

namespace Coursenix.Models
{
    public class Group
    {
        [Key] // Designates GroupId as the primary key
        public int GroupId { get; set; }

        // Foreign key relating this group to a Subject
        [ForeignKey("Subject")] // Specifies the navigation property name
        public int SubjectId { get; set; }

        // Navigation property to the parent Subject entity
        public Subject Subject { get; set; }

        // --- REMOVED: Direct link to Teacher (TeacherId and Teacher navigation property) ---
        // Removing these properties removes the FK_Groups_Teachers_TeacherId constraint.
        // This should resolve the "multiple cascade paths" error.
        // The Group is still indirectly linked to a Teacher via the Subject.Teacher relationship.
        // --- End REMOVED ---


        [Required]
        [Range(1, 12, ErrorMessage = "Grade must be between 1 and 12.")]
        public int Grade { get; set; }

        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string GroupName { get; set; } // Group name can be optional if not [Required]

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

        // Current count of enrolled students in this group
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Enrolled students count cannot be negative.")]
        public int EnrolledStudentsCount { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }
    }
}