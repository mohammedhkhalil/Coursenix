// Models/ViewModels/TeacherDashboardViewModel.cs
using Coursenix.Models; 
using System.Collections.Generic;

namespace Coursenix.Models.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public Teacher Teacher { get; set; }

        // Dashboard Statistics
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalGroups { get; set; } 

        public List<Subject> CoursesTaught { get; set; }

    }
}