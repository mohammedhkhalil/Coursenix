// Models/ViewModels/CourseDetailsViewModel.cs
using Coursenix.Models; // لاستخدام نماذج Subject, Teacher, Group
using System.Collections.Generic; // لاستخدام List
using System.ComponentModel.DataAnnotations; // لاستخدام Validation Attributes if needed

namespace Coursenix.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Course Subject { get; set; }

        public List<Group> Groups { get; set; }

        [Display(Name = "Selected Group")]
        public int SelectedGroupId { get; set; }

        public bool RequestSuccessful { get; set; } = false;
        public string RequestMessage { get; set; }

    }
}