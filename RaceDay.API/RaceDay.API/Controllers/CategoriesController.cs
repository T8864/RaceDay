using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _context.Categories.Select(c => new
            {
                c.CategoryID,
                c.CategoryName,
                c.Description
            }).ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult Create([FromBody] CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return StatusCode(201, new { message = "Category created successfully", categoryID = category.CategoryID });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult Update(int id, [FromBody] CreateCategoryDto dto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            category.CategoryName = dto.CategoryName ?? category.CategoryName;
            category.Description = dto.Description ?? category.Description;

            _context.SaveChanges();

            return Ok(new { message = "Category updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}