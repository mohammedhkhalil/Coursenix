namespace Coursenix.ViewModels
{
    public class CreateGroupVM
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public string GroupName { get; set; }
        public List<string> SelectedDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalSeats { get; set; }
        public string Description { get; set; }
    }
}
