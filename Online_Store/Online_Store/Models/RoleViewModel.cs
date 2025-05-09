using Microsoft.EntityFrameworkCore;
using Online_Store.Models;
using System.ComponentModel.DataAnnotations;
namespace Online_Store.Models
{
    public class RoleViewModel
    {
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role name is required.")]
        public string RoleName { get; set; }
    }
}
