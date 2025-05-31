using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class TakeAttendanceViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public string Days { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        public List<StudentAttendanceItem> Students { get; set; } = new List<StudentAttendanceItem>();
    }

    public class StudentAttendanceItem
    {

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool IsPresent { get; set; }
    }

    public class StudentAttendanceViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public string Days { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public List<StudentAttendanceRecord> Students { get; set; } = new List<StudentAttendanceRecord>();
    }

    public class StudentAttendanceRecord
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string PhoneNumber { get; set; }
        public string ParentPhoneNumber { get; set; }
        public double AbsencePercentage { get; set; }

        public string AbsenceClass => AbsencePercentage switch
        {
            >= 50 => "high",
            >= 25 => "medium",
            _ => "low"
        };
    }

    public class AttendanceSuccessViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string CourseName { get; set; }
        public int Grade { get; set; }
        public string Days { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime SessionDate { get; set; }

        public List<string> PresentStudents { get; set; } = new List<string>();
        public List<string> AbsentStudents { get; set; } = new List<string>();
    }
}