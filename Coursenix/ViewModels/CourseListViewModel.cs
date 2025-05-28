// ViewModels/CourseListViewModel.cs
using System.Collections.Generic;
using Coursenix.Models;

namespace Coursenix.Models.ViewModels
{
    public class CourseListViewModel
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<string> AvailableSubjects { get; set; } = new List<string>();
        public List<int> AvailableGrades { get; set; } = new List<int>();
        public string SelectedSubject { get; set; } = "";
        public int SelectedGrade { get; set; } = 0;
        public string SearchQuery { get; set; } = "";

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCourses { get; set; } = 0;

        // Helper properties for pagination
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int PreviousPage => CurrentPage - 1;
        public int NextPage => CurrentPage + 1;
    }
}