using Coursenix.Models; // Assuming your models are in this namespace
using System.Collections.Generic;

namespace Coursenix.Models.ViewModels
{
    public class StudentAttendanceViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CourseName { get; set; } // Assuming you link GradeLevel to Course
        public int GradeLevelNumber { get; set; } // Assuming you link GradeLevel to a number

        public List<string> GroupDays { get; set; } = new List<string>();
        public TimeSpan GroupStartTime { get; set; }
        public TimeSpan GroupEndTime { get; set; }

        public string? SearchTerm { get; set; }

        public List<StudentAttendanceDetailVM> Students { get; set; } = new List<StudentAttendanceDetailVM>();
    }

    public class StudentAttendanceDetailVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ParentPhoneNumber { get; set; }
        public decimal AbsencePercentage { get; set; } // Calculated absence percentage
    }
}