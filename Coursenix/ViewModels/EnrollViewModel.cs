namespace Coursenix.ViewModels
{
    public class EnrollViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }
        //public string? TeacherProfilePicture { get; set; }

        public List<GradeLevelViewModel> GradeLevels { get; set; }
    }

    public class GradeLevelViewModel
    {
        public int GradeLevelId { get; set; }
        public int NumberOfGrade { get; set; }
        public decimal Price { get; set; }
        public int NumberOfClassesPerWeek { get; set; }
        public List<GroupsGradeLevel> Groups { get; set; }
    }

    public class GroupsGradeLevel
    {
        public int GroupId { get; set; }
        public string? Name { get; set; }
        public List<string> SelectedDays { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalSeats { get; set; }
        public int EnrolledStudentsCount { get; set; }
    }

}
