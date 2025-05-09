using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Online_Store.Models
{
    public class AssignRoleViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; }

        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role name is required.")]
        public string RoleName { get; set; }

        public List<string>? AvailableRoles { get; set; }  // قائمة الأدوار المتاحة للمستخدم لاختيارها
    }
}
