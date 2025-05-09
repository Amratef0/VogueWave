using System.ComponentModel.DataAnnotations;

namespace Online_Store.Models
{
    public class ProfileViewModel
    {

        [Required(ErrorMessage = "Address is required")]

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Username")]
        public string? UserName { get; set; } // للعرض فقط
    }
}
