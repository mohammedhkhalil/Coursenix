namespace Coursenix.ViewModels
{
    public class EnrollViewModel
    {
        public string? Biography { get; set; }
        public string Name { get; set; }
        public string TeacherName { get; set; }
        public string? Location { get; set; }
        public List<Grades> grades { get; set; }
        public class Grades
        {
            public int number { get; set; }
            public int Id { get; set; }
        }
    }
}
