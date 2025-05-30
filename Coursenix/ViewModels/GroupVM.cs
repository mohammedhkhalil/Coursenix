using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class GroupVM
    {
        [Required(ErrorMessage = "Group name is required")]
        [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters")]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be between 0.01 and 9999.99")]
        [Display(Name = "Price ($)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "At least one day must be selected")]
        [Display(Name = "Days")]
        public List<string> Days { get; set; } = new List<string>();

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        // Validation: End time should be after start time
        public bool IsValidTimeRange
        {
            get
            {
                return EndTime > StartTime;
            }
        }
    }
}
