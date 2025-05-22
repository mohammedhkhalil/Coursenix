using Microsoft.AspNetCore.Identity;

namespace Coursenix.Models
{
    public class AppUser : IdentityUser
    {
        // add any additional properties you need for your user
        public string Name { get; set; }
        public string? RoleType { get; set; } // "Student", "Teacher"

    }

}
