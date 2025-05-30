using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Coursenix.ViewModels
{
    public class EditCourseVM
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(255, ErrorMessage = "Course name cannot exceed 255 characters")]
        [Display(Name = "Course Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Course description cannot exceed 1000 characters")]
        [Display(Name = "Course Description")]
        public string? Description { get; set; }

        [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
        [Display(Name = "Course Location")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Students per group is required")]
        [Range(1, 500, ErrorMessage = "Students per group must be between 1 and 500")]
        [Display(Name = "Maximum Students Per Group")]
        public int StudentsPerGroup { get; set; }

        [Display(Name = "Course Thumbnail")]
        public IFormFile? ThumbnailFile { get; set; }
        public string? CurrentThumbnailUrl { get; set; }
        public bool RemoveThumbnail { get; set; } = false;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int TotalGroups { get; set; }
        public int TotalEnrollments { get; set; }

        // Constructor
        public EditCourseVM()
        {
            StudentsPerGroup = 10; // Default value
        }
        public List<GradeLevelVM> GradeLevels { get; set; } = new List<GradeLevelVM>();

    }
}