namespace Coursenix.ViewModels
{
    public class StudentGroupVM
    {
        public int GroupId { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; } // اختياري
        public string DayOfWeek { get; set; }
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public string Location { get; set; }
        public double AbsencePercentage { get; set; } // نسبة الغياب
    }

}
