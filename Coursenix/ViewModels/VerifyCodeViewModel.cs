using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        [StringLength(1)]
        public string Digit1 { get; set; }

        [Required]
        [StringLength(1)]
        public string Digit2 { get; set; }

        [Required]
        [StringLength(1)]
        public string Digit3 { get; set; }

        [Required]
        [StringLength(1)]
        public string Digit4 { get; set; }

        [Required]
        [StringLength(1)]
        public string Digit5 { get; set; }

        [Required]
        [StringLength(1)]
        public string Digit6 { get; set; }
    }
}
