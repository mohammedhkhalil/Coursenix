// Models/ViewModels/CourseListViewModel.cs
using System.Collections.Generic;
using Coursenix.Models;

namespace Coursenix.ViewModels
{
    public class CourseListViewModel
    {
        public List<Subject> Courses { get; set; }

        public List<string> AvailableSubjects { get; set; }
        public List<int> AvailableGrades { get; set; }
        public string SelectedSubject { get; set; }
        public int? SelectedGrade { get; set; }
        public string SearchQuery { get; set; }
    }
}