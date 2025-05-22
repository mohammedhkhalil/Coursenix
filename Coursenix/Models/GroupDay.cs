using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Coursenix.Enums;

namespace Coursenix.Models
{
    [Table("GroupDays")]
    public class GroupDay
    {
        [Key]
        public int GroupDayId { get; set; }

        [Required]
        public WeekDay Day { get; set; }    

        // FK → Group
        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
