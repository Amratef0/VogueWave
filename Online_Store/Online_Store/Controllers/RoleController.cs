using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Online_Store.Models;
using System.Threading.Tasks;

namespace Online_Store.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult AddRole()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveRole(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole();

                role.Name = roleViewModel.RoleName;
                IdentityResult result = await _roleManager.CreateAsync(role);

            
                if (result.Succeeded)
                {
                    ViewBag.Success = "Role created successfully!";
                    return RedirectToAction("AddRole"); 
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View("AddRole", roleViewModel); 
        }


        [HttpGet]
        public IActionResult AssignRole()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new AssignRoleViewModel
            {
                AvailableRoles = roles
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveAssignRole(AssignRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("AssignRole", model);
                }

                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    TempData["Error"] = "User not found!";
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("AssignRole", model);
                }

                if (await _userManager.IsInRoleAsync(user, model.RoleName))
                {
                    TempData["Error"] = $"User already has the '{model.RoleName}' role.";
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("AssignRole", model);
                }

                var result = await _userManager.AddToRoleAsync(user, model.RoleName);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role assigned successfully!";
                    return RedirectToAction("AssignRole");
                }

                TempData["Error"] = "Failed to assign role!";
                model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View("AssignRole", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while assigning the role.";
                model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View("AssignRole", model);
            }
            
        }

        public IActionResult RemoveRoleFromUser()
        {
            var model = new AssignRoleViewModel
            {
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };
            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveRemoveRoleFromUser(AssignRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("RemoveRoleFromUser", model);  
                }

                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    TempData["Error"] = "User not found!";
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("RemoveRoleFromUser", model);  
                }

                if (!await _userManager.IsInRoleAsync(user, model.RoleName))
                {
                    TempData["Error"] = "User is not assigned to this role.";
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View("RemoveRoleFromUser", model); 
                }

                var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role removed from user successfully!";
                    return View("RemoveRoleFromUser", model); 
                }

                TempData["Error"] = "Failed to remove role!";
                model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View("RemoveRoleFromUser", model);  
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while removing the role.";
                model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View("RemoveRoleFromUser", model); 
            }
        }

    }


}






