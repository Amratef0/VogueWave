using Microsoft.AspNetCore.Identity;

namespace Online_Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>(); 

        public string? Address { get; set; }
    }
}
