using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;
using System.Threading.Tasks;

public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> ViewProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Address = user.Address
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ProfileViewModel
        {
            Address = user.Address
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEditProfile(ProfileViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View("EditProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("ViewProfile");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                return View("EditProfile", model);
            }
        }
        catch (Exception ex)
        {
            // Log the error (you can log it to a file, a logging system, or whatever you prefer)
            TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
            return View("EditProfile", model);
        }
    }

    [HttpGet]
    public IActionResult ChangeProfilePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveChangeProfilePassword(string currentPassword, string newPassword)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("ViewProfile");
        }

        TempData["ErrorMessage"] = "Current Password is wrong.";
        return View("ChangeProfilePassword");
    }
}
