using System.ComponentModel.DataAnnotations;

namespace Online_Store.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
