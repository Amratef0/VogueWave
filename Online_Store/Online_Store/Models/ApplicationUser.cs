using Microsoft.AspNetCore.Identity;

namespace Online_Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>(); // هذا يعني أن المستخدم يمكنه أن يكون له العديد من الطلبات

        public string? Address { get; set; }
    }
}
