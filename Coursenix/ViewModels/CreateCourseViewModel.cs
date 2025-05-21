using System.ComponentModel.DataAnnotations;
using Coursenix.ViewModels;

namespace Coursenix.Models.ViewModels
{
    public class CreateCourseViewModel
    {
        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; } = false;

        [Required(ErrorMessage = "Course title is required.")]
        [Display(Name = "Course Title")]
        [StringLength(255, ErrorMessage = "Course title cannot exceed 255 characters.")]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Display(Name = "Price ($)")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        [DataType(DataType.Currency)]
        public int CoursePrice { get; set; }

        [Display(Name = "Course Thumbnail")]
        public IFormFile? ThumbnailFile { get; set; }// will make the images by defult in the system 

        [Required(ErrorMessage = "Course description is required.")]
        [Display(Name = "Course Description")]
        [StringLength(2000, ErrorMessage = "Course description cannot exceed 2000 characters.")]
        public string CourseDescription { get; set; }

        [Required(ErrorMessage = "Grade level is required.")]
        [Display(Name = "Grade Level")]
        [Range(7, 12, ErrorMessage = "Grade level must be between 7 and 12.")]
        public int CourseGradeLevel { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [Display(Name = "Location")]
        [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Students per group is required.")]
        [Display(Name = "Students/Group")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum 1 student per group.")]
        public int StudentsPerGroup { get; set; }

        public List<AddGroupsViewModel>? Groups { get; set; }
    }

}