// Models/ViewModels/GroupViewModel.cs - Dedicated file

using System;
using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class GroupViewModel
    {
        [Display(Name = "Group Name")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Days are required.")]
        [Display(Name = "Days")]
        [StringLength(50, ErrorMessage = "Days string cannot exceed 50 characters.")]
        public string Days { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Time From")]
        [DataType(DataType.Time)]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [Display(Name = "Time To")]
        [DataType(DataType.Time)]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "Grade is required for the group.")]
        [Range(1, 12, ErrorMessage = "Grade must be between 1 and 12.")]
        public int Grade { get; set; }

        // Note: Properties like SubjectId, Subject, TotalSeats, EnrolledStudentsCount, Location
        // belong to the Group entity model and the AddGroupsViewModel, NOT the GroupViewModel.
    }
}