using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var firstName = HttpContext.Session.GetString("FirstName");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            ViewBag.FirstName = firstName;

            if (role == "Organiser")
                return View("Organiser");

            return View("Participant");
        }
    }
}