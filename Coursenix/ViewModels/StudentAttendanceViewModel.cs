namespace Coursenix.ViewModels
{
    public class GroupAttendanceViewModel
    {
        // Group Info
        public int GradeLevel { get; set; }
        public string GroupName { get; set; }
        public List<string> Days { get; set; } = new List<string>();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Students
        public List<StudentAttendance> Students { get; set; }

        public class StudentAttendance
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string ParentPhoneNumber { get; set; }
            public double AbsencePercentage { get; set; }
        }
    }
}
