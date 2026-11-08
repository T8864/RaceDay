using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public CategoriesController(IHttpClientFactory factory)
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
            var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>("api/Categories");
            return View(categories ?? new List<CategoryViewModel>());
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Categories", new
            {
                categoryName = model.CategoryName,
                description = model.Description
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create category.");
                return View(model);
            }

            TempData["Success"] = "Category created.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            var cat = await client.GetFromJsonAsync<CategoryViewModel>($"api/Categories/{id}");
            if (cat == null) return NotFound();

            return View(new CategoryFormViewModel
            {
                CategoryID = cat.CategoryID,
                CategoryName = cat.CategoryName,
                Description = cat.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            if (!ModelState.IsValid) return View(model);

            var client = GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Categories/{id}", new
            {
                categoryName = model.CategoryName,
                description = model.Description
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update category.");
                return View(model);
            }

            TempData["Success"] = "Category updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Dashboard");

            var client = GetAuthorizedClient();
            await client.DeleteAsync($"api/Categories/{id}");
            TempData["Success"] = "Category deleted.";
            return RedirectToAction("Index");
        }
    }
}