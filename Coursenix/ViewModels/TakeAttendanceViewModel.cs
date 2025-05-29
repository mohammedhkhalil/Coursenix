namespace Coursenix.Models.ViewModels
{
    public class TakeAttendanceViewModel
    {
        public int SessionId { get; set; }
        public DateTime SessionDateTime { get; set; }
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