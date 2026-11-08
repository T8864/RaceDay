using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AccountController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _factory.CreateClient("Api");
            var request = new
            {
                email = model.Email,
                password = model.Password
            };

            var response = await client.PostAsJsonAsync("api/Auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (login == null || string.IsNullOrWhiteSpace(login.Token))
            {
                ModelState.AddModelError("", "Login failed. Please try again.");
                return View(model);
            }

            HttpContext.Session.SetString("Token", login.Token);
            HttpContext.Session.SetString("Role", login.Role);
            HttpContext.Session.SetString("FirstName", login.FirstName);
            HttpContext.Session.SetInt32("UserID", login.UserID);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _factory.CreateClient("Api");
            var request = new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                email = model.Email,
                password = model.Password,
                role = model.Role,
                phone = model.Phone
            };

            var response = await client.PostAsJsonAsync("api/Auth/register", request);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Registration failed. Email may already be in use.");
                return View(model);
            }

            TempData["Success"] = "Account created. Please log in.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}