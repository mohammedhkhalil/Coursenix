using Coursenix.Models;

namespace Coursenix.ViewModels
{
    public class CourseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailFileName { get; set; }
        public string GradeRange { get; set; }
    }
}
