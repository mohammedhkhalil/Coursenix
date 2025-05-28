using Coursenix.Models;
using System.Collections.Generic;

namespace Coursenix.ViewModels
{
    public class TeacherDashboardVM
    {
        public string TeacherName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalGroups { get; set; }
        public string Biography { get; set; }
        public string PP {  get; set; }
        public List<CourseInfo> Courses { get; set; }
    }
}