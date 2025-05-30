using System;
using System.Collections.Generic;

namespace Coursenix.ViewModels
{
   
    public class StudentGroupViewModel
    {
        /* ----- Student info ----- */
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        /* ----- Group / Subject / Teacher ----- */
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }

        /* ----- Schedule ----- */
        public List<string> Days { get; set; } = new();

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }

        /* ----- Attendance ----- */
        public int TotalSessions { get; set; }
        public int SessionsAttended { get; set; }

        public double AttendancePercentage =>
            TotalSessions == 0
                ? 0
                : Math.Round((double)SessionsAttended / TotalSessions * 100, 2);

        public double AbsencePercentage => 100 - AttendancePercentage;
    }
}
