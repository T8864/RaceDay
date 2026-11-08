using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ResultsController(IHttpClientFactory factory)
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
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var results = await client.GetFromJsonAsync<List<ResultViewModel>>("api/Results");
            return View(results ?? new List<ResultViewModel>());
        }

        public async Task<IActionResult> MyResults()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login", "Account");

            var client = GetAuthorizedClient();
            var results = await client.GetFromJsonAsync<List<ResultViewModel>>("api/Results/my");
            return View(results ?? new List<ResultViewModel>());
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");
            return View(new ResultFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Results", new
            {
                enrolmentID = model.EnrolmentID,
                finishTime = model.FinishTime,
                position = model.Position,
                notes = model.Notes
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to post result.");
                return View(model);
            }

            TempData["Success"] = "Result posted.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var result = await client.GetFromJsonAsync<ResultViewModel>($"api/Results/{id}");
            if (result == null) return NotFound();

            return View(new ResultFormViewModel
            {
                ResultID = result.ResultID,
                EnrolmentID = result.EnrolmentID,
                FinishTime = result.FinishTime,
                Position = result.Position,
                Notes = result.Notes
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResultFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Results/{id}", new
            {
                finishTime = model.FinishTime,
                position = model.Position,
                notes = model.Notes
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update result.");
                return View(model);
            }

            TempData["Success"] = "Result updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            await client.DeleteAsync($"api/Results/{id}");
            TempData["Success"] = "Result deleted.";
            return RedirectToAction("Index");
        }
    }
}