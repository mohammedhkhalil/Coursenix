using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class EditGroupVM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int GradeLevelId { get; set; }
        public int GroupNumberInGrade { get; set; }

        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
        [Display(Name = "Group Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please select at least one day")]
        [Display(Name = "Selected Days")]
        public List<string> SelectedDays { get; set; } = new List<string>();

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Total seats is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1")]
        [Display(Name = "Total Seats")]
        public int TotalSeats { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        [Display(Name = "Group Location")]
        public string? Location { get; set; }

        // Readonly properties for context
        public string CourseName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int GradeNumber { get; set; }
        public int EnrolledStudentsCount { get; set; }
        public int AvailableSeats => TotalSeats - EnrolledStudentsCount;

        // Available days for selection
        public List<DayOption> AvailableDays { get; set; } = new List<DayOption>
        {
            new DayOption { Value = "Sunday", Text = "Sunday" },
            new DayOption { Value = "Monday", Text = "Monday" },
            new DayOption { Value = "Tuesday", Text = "Tuesday" },
            new DayOption { Value = "Wednesday", Text = "Wednesday" },
            new DayOption { Value = "Thursday", Text = "Thursday" },
            new DayOption { Value = "Friday", Text = "Friday" },
            new DayOption { Value = "Saturday", Text = "Saturday" }
        };

        // Enrolled students for display
        public List<EnrolledStudent> EnrolledStudents { get; set; } = new List<EnrolledStudent>();
    }

    public class DayOption
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class EnrolledStudent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }
}