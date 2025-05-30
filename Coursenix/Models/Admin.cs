using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coursenix.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AppUserId { get; set; } // Foreign key to AppUser
        public AppUser AppUser { get; set; } // Navigation property
        // Any admin-specific properties could be added here

    }
}
