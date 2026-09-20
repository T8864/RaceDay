using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResultsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var results = _context.Results.Select(r => new
            {
                r.ResultID,
                r.EnrolmentID,
                r.FinishTime,
                r.PositionOverall,
                r.PositionCategory,
                r.Notes
            }).ToList();

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _context.Results.FirstOrDefault(r => r.ResultID == id);
            if (result == null)
                return NotFound(new { message = "Result not found" });

            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public IActionResult GetMyResults()
        {
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var results = _context.Results
                .Where(r => r.Enrolment.UserID == userID)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrolmentID,
                    r.FinishTime,
                    r.PositionOverall,
                    r.PositionCategory,
                    r.Notes
                }).ToList();

            return Ok(results);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult Create([FromBody] CreateResultDto dto)
        {
            var enrolment = _context.Enrolments.FirstOrDefault(e => e.EnrolmentID == dto.EnrolmentID);
            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            var existing = _context.Results.FirstOrDefault(r => r.EnrolmentID == dto.EnrolmentID);
            if (existing != null)
                return StatusCode(409, new { message = "Result already exists for this enrolment" });

            var result = new Result
            {
                EnrolmentID = dto.EnrolmentID,
                FinishTime = dto.FinishTime ?? string.Empty,
                PositionOverall = dto.PositionOverall,
                PositionCategory = dto.PositionCategory,
                Notes = dto.Notes
            };

            _context.Results.Add(result);
            _context.SaveChanges();

            return StatusCode(201, new { message = "Result added successfully", resultID = result.ResultID });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult Update(int id, [FromBody] CreateResultDto dto)
        {
            var result = _context.Results.FirstOrDefault(r => r.ResultID == id);
            if (result == null)
                return NotFound(new { message = "Result not found" });

            result.FinishTime = dto.FinishTime ?? result.FinishTime;
            result.PositionOverall = dto.PositionOverall ?? result.PositionOverall;
            result.PositionCategory = dto.PositionCategory ?? result.PositionCategory;
            result.Notes = dto.Notes ?? result.Notes;

            _context.SaveChanges();

            return Ok(new { message = "Result updated successfully" });
        }
    }
}