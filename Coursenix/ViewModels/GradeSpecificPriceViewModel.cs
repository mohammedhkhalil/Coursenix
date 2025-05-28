using System.ComponentModel.DataAnnotations;

namespace Coursenix.ViewModels
{
    public class GradeSpecificPriceViewModel
    {

        [Range(7, 12)]
        public int GradeLevel { get; set; }

        [Range(1, int.MaxValue)]
        public int Price { get; set; }
    }

}
