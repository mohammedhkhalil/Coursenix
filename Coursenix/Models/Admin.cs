using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        // Any admin-specific properties could be added here

    }
}
