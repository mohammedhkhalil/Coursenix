// Models/ViewModels/CourseDetailsViewModel.cs
using Coursenix.Models; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;

namespace Coursenix.Models.ViewModels
{
    public class CourseDetailsViewModel
    {
        public Subject Subject { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Group> Groups { get; set; }

        [Display(Name = "Selected Group")]
        public int SelectedGroupId { get; set; } 

        public bool RequestSuccessful { get; set; } = false;
        public string RequestMessage { get; set; }

    }
}