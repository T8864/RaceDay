using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrolmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return StatusCode(403, new { message = "Forbidden - Organiser role required" });

            var enrolments = _context.Enrolments.Select(e => new
            {
                e.EnrolmentID,
                e.UserID,
                e.EventID,
                e.CategoryID,
                e.EnrolmentDate,
                e.Status
            }).ToList();

            return Ok(enrolments);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return Unauthorized(new { message = "Login required" });

            var enrolment = _context.Enrolments.FirstOrDefault(e => e.EnrolmentID == id);
            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            return Ok(enrolment);
        }

        [HttpGet("my")]
        public IActionResult GetMyEnrolments()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return Unauthorized(new { message = "Login required" });

            var enrolments = _context.Enrolments
                .Where(e => e.UserID == userID)
                .Select(e => new
                {
                    e.EnrolmentID,
                    e.EventID,
                    e.CategoryID,
                    e.EnrolmentDate,
                    e.Status
                }).ToList();

            return Ok(enrolments);
        }

        [HttpPost]
        public IActionResult Enrol([FromBody] CreateEnrolmentDto dto)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return StatusCode(403, new { message = "Forbidden - Participant role required" });

            var userID = HttpContext.Session.GetInt32("UserID");

            var existing = _context.Enrolments
                .FirstOrDefault(e => e.UserID == userID && e.EventID == dto.EventID);
            if (existing != null)
                return StatusCode(409, new { message = "Already enrolled in this event" });

            var eventItem = _context.Events.FirstOrDefault(e => e.EventID == dto.EventID);
            if (eventItem == null)
                return NotFound(new { message = "Event not found" });

            var currentCount = _context.Enrolments.Count(e => e.EventID == dto.EventID);
            if (currentCount >= eventItem.MaxParticipants)
                return BadRequest(new { message = "Event is fully booked" });

            var enrolment = new Enrolment
            {
                UserID = userID!.Value,
                EventID = dto.EventID,
                CategoryID = dto.CategoryID,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            _context.SaveChanges();

            return StatusCode(201, new { message = "Enrolment successful", enrolmentID = enrolment.EnrolmentID });
        }

        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return Unauthorized(new { message = "Login required" });

            var enrolment = _context.Enrolments.FirstOrDefault(e => e.EnrolmentID == id);
            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found" });

            if (enrolment.UserID != userID)
                return StatusCode(403, new { message = "Forbidden - you can only cancel your own enrolment" });

            _context.Enrolments.Remove(enrolment);
            _context.SaveChanges();

            return Ok(new { message = "Enrolment cancelled successfully" });
        }
    }
}