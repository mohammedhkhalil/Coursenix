using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models.ViewModels
{
    public class TakeAttendanceViewModel
    {
        public int SessionId { get; set; }
        //search
        public string? SearchTerm { get; set; }

        public DateTime SessionDateTime { get; set; }
        [Column(TypeName = "time")]
        public DateTime GStartTime { get; set; }

        [Column(TypeName = "time")]
        public DateTime GEndTime { get; set; }
        public List<string> Days { get; set; } = new List<string>();
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public List<StudentAttendanceVM> Students { get; set; }
    }

    public class StudentAttendanceVM
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }

        //from attendance
        public bool IsPresent { get; set; }
    }

}
