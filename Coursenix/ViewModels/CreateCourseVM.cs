using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class CreateCourseVM
    {
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(255, ErrorMessage = "Course title cannot exceed 255 characters")]
        [Display(Name = "Course Title")]
        public string CourseTitle { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Course Description")]
        public string? CourseDescription { get; set; }

        [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Students per group is required")]
        [Range(1, 1000, ErrorMessage = "Students per group must be between 1 and 1000")]
        [Display(Name = "Students Per Group")]
        public int StudentsPerGroup { get; set; }

        [Display(Name = "Course Thumbnail")]
        public IFormFile? ThumbnailFile { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        // Dictionary to hold grade numbers as keys and their groups as values
        // Key: Grade number (7-12), Value: List of groups for that grade
        public Dictionary<int, List<GroupVM>> GradeGroups { get; set; } = new Dictionary<int, List<GroupVM>>();

        // For validation - ensure at least one grade is selected
        public bool HasSelectedGrades => GradeGroups.Any(kvp => kvp.Value.Any());
    }
}
