using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class GradeLevelVM
{
    public int? Id { get; set; } // Nullable for new grade levels (0 indicates new)
    [Required(ErrorMessage = "Grade number is required.")]
    [Range(1, 12, ErrorMessage = "Grade number must be between 1 and 12.")]
    [DisplayName("Grade Number")]
    public int NumberOfGrade { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public decimal Price { get; set; }

    // This flag is used client-side to mark items for removal without deleting them immediately from the DOM.
    // The server-side logic will interpret this for deletion.
    public bool IsDeleted { get; set; } = false;
}