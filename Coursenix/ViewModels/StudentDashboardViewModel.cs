using System;
using System.Collections.Generic;

namespace Coursenix.Models
{
    public class StudentDashboardViewModel
    {
        public class CourseInfo
        {
            public string CourseName { get; set; }
            public string TeacherName { get; set; }
            public string GroupName { get; set; }
            public string Location { get; set; }
            public string Days { get; set; } 
            public string Time { get; set; } 
            public double AbsenceRatio { get; set; } // Percentage
            public string AbsenceClass { get; set; } // "low", "medium", "high"
        }

        public string StudentName { get; set; }
        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
    }
}