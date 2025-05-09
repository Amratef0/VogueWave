using System.ComponentModel.DataAnnotations;
namespace Online_Store.Models

{
    public class LoginUserViewModel
    {
       
            [Required]
            [Display(Name = "Username or Email")]
            public string UserNameOrEmail { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        

    }
}
