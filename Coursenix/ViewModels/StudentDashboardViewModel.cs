using System;
using System.Collections.Generic;

namespace Coursenix.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } // For user profile display
        public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();

        public class CourseInfo
        {
            public string CourseName { get; set; }
            public string TeacherName { get; set; }
            public string GroupName { get; set; }
            public string Location { get; set; }
            public List<string> Days { get; set; }
            public string TimeRange { get; set; }
            public string AbsenceRatio { get; set; }
            public string AbsenceClass { get; set; } // For CSS: "low", "medium", etc.
        }
    }
}