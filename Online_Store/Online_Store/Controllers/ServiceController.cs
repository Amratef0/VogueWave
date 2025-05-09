using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Online_Store.Models;
using Microsoft.AspNetCore.Authorization;

namespace Online_Store.Controllers
{

    public class ServiceController : Controller
    {
        [Authorize]

        public IActionResult TestAuth()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                Claim IdClaim = User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier);
                Claim AddressClaim = User.Claims.FirstOrDefault(c => c.Type == "User Address");
                string id = IdClaim.Value;
                string name = User.Identity.Name;
                return Content($"Welcome{name} \n ID = {id}");
            }
            return Content("Welcome Guest");
        }
    }
}
