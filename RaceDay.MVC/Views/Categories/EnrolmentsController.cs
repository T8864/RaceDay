using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public EnrolmentsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient GetAuthorizedClient()
        {
            var client = _factory.CreateClient("Api");
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> MyEnrolments()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login", "Account");

            var client = GetAuthorizedClient();
            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>("api/Enrolments/my");
            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        public async Task<IActionResult> Enrol(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var ev = await client.GetFromJsonAsync<EventViewModel>($"api/Events/{eventId}");
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrolConfirm(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Enrolments", new
            {
                eventID = eventId
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = error.Contains("already enrolled")
                    ? "You are already enrolled in this event."
                    : error.Contains("full")
                    ? "This event is full."
                    : "Enrolment failed. Please try again.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            TempData["Success"] = "Successfully enrolled in the event.";
            return RedirectToAction("MyEnrolments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            await client.DeleteAsync($"api/Enrolments/{id}");
            TempData["Success"] = "Enrolment cancelled.";
            return RedirectToAction("MyEnrolments");
        }

        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>($"api/Enrolments/event/{eventId}");
            ViewBag.EventId = eventId;
            return View(enrolments ?? new List<EnrolmentViewModel>());
        }
    }
}