using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models.ViewModels
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Course Title is required")]
        [MaxLength(255)]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal CoursePrice { get; set; }

        [MaxLength(1000)]
        public string CourseDescription { get; set; }

        // HTML input is text, but Subject model expects int.
        // Using string here to match HTML, will need parsing/validation in controller.
        // A better HTML design would be a number input or dropdown for GradeLevel.
        [Required(ErrorMessage = "Grade Levels are required")]
        public string GradeLevels { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        // This field from HTML doesn't map directly to Subject or Group in a simple way.
        // It implies group *creation* logic based on this number, which is more complex.
        // We'll include it in the DTO but note its ambiguity relative to current models.
        public int StudentsPerGroup { get; set; }

        // For file upload
        public IFormFile ThumbnailFile { get; set; }

        // TeacherId is needed for the Subject model but not in the form.
        // This will need to be obtained from the authenticated user in the controller.
        // It's not a property of the ViewModel receiving form data.
    }
}