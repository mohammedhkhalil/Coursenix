// Models/ViewModels/TeacherDashboardViewModel.cs
using Coursenix.Models;
using System.Collections.Generic;

namespace Coursenix.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public Teacher Teacher { get; set; }

        // Dashboard Statistics
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalGroups { get; set; }

        public List<Course> CoursesTaught { get; set; }

    }
}