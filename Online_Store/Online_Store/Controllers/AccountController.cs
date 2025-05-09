using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Online_Store.Interface;
using System.Net;
using System.Web;
using NuGet.Common;

namespace Online_Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Online_Store.Interface.IEmailSender _emailSender;


        public AccountController(UserManager<ApplicationUser> userManager,
                         SignInManager<ApplicationUser> signInManager,
                         Online_Store.Interface.IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRegister(RegisterViewModel UserViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = new ApplicationUser
                {
                    Address = UserViewModel.Address,
                    UserName = UserViewModel.UserName,
                    Email = UserViewModel.Email // إضافة الإيميل هنا
                };

                // إنشاء المستخدم في النظام
                IdentityResult result = await _userManager.CreateAsync(appuser, UserViewModel.Password);

                if (result.Succeeded)
                {
                    // تسجيل الدخول للمستخدم بعد التسجيل بنجاح
                    await _signInManager.SignInAsync(appuser, isPersistent: false);
                    return RedirectToAction("Index", "Main");
                }

                // في حالة حدوث أي خطأ أثناء عملية التسجيل
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View("Register", UserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLogin(LoginUserViewModel UserViewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = null;

                // التحقق إذا كان المدخل بريد إلكتروني أم اسم مستخدم
                if (UserViewModel.UserNameOrEmail.Contains("@"))
                {
                    appuser = await _userManager.FindByEmailAsync(UserViewModel.UserNameOrEmail);
                }
                else
                {
                    appuser = await _userManager.FindByNameAsync(UserViewModel.UserNameOrEmail);
                }

                if (appuser != null)
                {
                    bool found = await _userManager.CheckPasswordAsync(appuser, UserViewModel.Password);
                    if (found)
                    {
                        List<Claim> Claims = new List<Claim>
                {
                    new Claim("UserAddress", appuser.Address)
                };
                        await _signInManager.SignInAsync(appuser, UserViewModel.RememberMe);
                        return RedirectToAction("Index", "Main");
                    }
                }
                ModelState.AddModelError("", "User Or Password Wrong");
            }
            return View("Login", UserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // إذا كان البريد الإلكتروني غير موجود، يمكن إضافة رسالة خطأ أو تقديم إشعار عام
                TempData["ErrorMessage"] = "Email not found!";
                return RedirectToAction("ForgotPassword");
            }

            // محاولة توليد التوكن
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            HttpUtility.UrlEncode(token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Error generating reset token!";
                return RedirectToAction("ForgotPassword");
            }

            // بناء رابط إعادة تعيين كلمة المرور
            var resetLink = $"https://localhost:7156/Account/ResetPassword?token={token}&email={user.Email}";

            try
            {
                // إرسال البريد الإلكتروني عبر خدمة البريد الإلكتروني
                await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
            }
            catch (Exception ex)
            {
                // التعامل مع الاستثناء في حالة فشل إرسال البريد الإلكتروني
                TempData["ErrorMessage"] = "Error sending email: " + ex.Message;
                return RedirectToAction("ForgotPassword");
            }

            return RedirectToAction("ForgotPasswordConfirmation");
        }


        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveResetPassword(ResetPasswordViewModel model)
        {
            model.Token = HttpUtility.UrlDecode(model.Token);
            Console.WriteLine($"Received Token: {model.Token}");

            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);  // إعادة عرض نفس الصفحة مع الأخطاء
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPassword"); // في حالة عدم وجود المستخدم
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                // عرض رسالة نجاح في نفس الصفحة
                ViewData["SuccessMessage"] = "Your password has been successfully reset!";
                return View("ResetPassword", model); // إعادة نفس الصفحة مع رسالة النجاح
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ResetPassword", model);  // إعادة نفس الصفحة مع الأخطاء
        }

    }

}

