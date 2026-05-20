using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Project> Projects { get; set; }
    }
}
