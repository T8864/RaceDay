using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public EventsController(IHttpClientFactory factory)
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

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login", "Account");

            var client = GetAuthorizedClient();
            var events = await client.GetFromJsonAsync<List<EventViewModel>>("api/Events");
            return View(events ?? new List<EventViewModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login", "Account");

            var client = GetAuthorizedClient();
            var ev = await client.GetFromJsonAsync<EventViewModel>($"api/Events/{id}");
            if (ev == null) return NotFound();
            return View(ev);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");
            return View(new EventFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Events", new
            {
                eventName = model.EventName,
                eventDate = model.EventDate,
                location = model.Location,
                distance = model.Distance,
                routeDescription = model.RouteDescription,
                maxParticipants = model.MaxParticipants,
                categoryID = model.CategoryID
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create event.");
                return View(model);
            }

            var created = await response.Content.ReadFromJsonAsync<CreateEventResponse>();

            if (model.ImageFile != null && model.ImageFile.Length > 0 && created != null)
            {
                using var content = new MultipartFormDataContent();
                using var stream = model.ImageFile.OpenReadStream();
                content.Add(new StreamContent(stream), "file", model.ImageFile.FileName);
                await client.PostAsync($"api/Events/{created.EventID}/upload-image", content);
            }

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var ev = await client.GetFromJsonAsync<EventViewModel>($"api/Events/{id}");
            if (ev == null) return NotFound();

            var model = new EventFormViewModel
            {
                EventID = ev.EventID,
                EventName = ev.EventName,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                RouteDescription = ev.RouteDescription,
                MaxParticipants = ev.MaxParticipants,
                CategoryID = ev.CategoryID,
                Status = ev.Status
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Events/{id}", new
            {
                eventName = model.EventName,
                eventDate = model.EventDate,
                location = model.Location,
                distance = model.Distance,
                status = model.Status,
                maxParticipants = model.MaxParticipants
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update event.");
                return View(model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var content = new MultipartFormDataContent();
                using var stream = model.ImageFile.OpenReadStream();
                content.Add(new StreamContent(stream), "file", model.ImageFile.FileName);
                await client.PostAsync($"api/Events/{id}/upload-image", content);
            }

            TempData["Success"] = "Event updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            await client.DeleteAsync($"api/Events/{id}");
            TempData["Success"] = "Event deleted.";
            return RedirectToAction("Index");
        }
    }
}