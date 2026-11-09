using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BlobService _blobService;

        public EventsController(AppDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var events = _context.Events.Select(e => new
            {
                e.EventID,
                e.EventName,
                e.EventDate,
                e.Location,
                e.Distance,
                e.Status,
                e.MaxParticipants,
                e.OrganiserID,
                e.CategoryID,
                e.ImageBlobUrl
            }).ToList();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var e = _context.Events.FirstOrDefault(e => e.EventID == id);
            if (e == null)
                return NotFound(new { message = "Event not found" });

            return Ok(e);
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult Create([FromBody] CreateEventDto dto)
        {
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                RouteDescription = dto.RouteDescription,
                MaxParticipants = dto.MaxParticipants,
                CategoryID = dto.CategoryID,
                OrganiserID = userID
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return StatusCode(201, new { message = "Event created successfully", eventID = newEvent.EventID });
        }

        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var e = _context.Events.FirstOrDefault(e => e.EventID == id);
            if (e == null)
                return NotFound(new { message = "Event not found" });

            if (e.OrganiserID != userID)
                return StatusCode(403, new { message = "Forbidden - you can only upload images for your own events" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var imageUrl = await _blobService.UploadImageAsync(file);
            e.ImageBlobUrl = imageUrl;
            _context.SaveChanges();

            return Ok(new { message = "Image uploaded successfully", imageUrl });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult Update(int id, [FromBody] UpdateEventDto dto)
        {
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var e = _context.Events.FirstOrDefault(e => e.EventID == id);
            if (e == null)
                return NotFound(new { message = "Event not found" });

            if (e.OrganiserID != userID)
                return StatusCode(403, new { message = "Forbidden - you can only update your own events" });

            e.EventName = dto.EventName ?? e.EventName;
            e.EventDate = dto.EventDate ?? e.EventDate;
            e.Location = dto.Location ?? e.Location;
            e.Distance = dto.Distance ?? e.Distance;
            e.Status = dto.Status ?? e.Status;
            e.MaxParticipants = dto.MaxParticipants ?? e.MaxParticipants;

            _context.SaveChanges();

            return Ok(new { message = "Event updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult Delete(int id)
        {
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var e = _context.Events.FirstOrDefault(e => e.EventID == id);
            if (e == null)
                return NotFound(new { message = "Event not found" });

            if (e.OrganiserID != userID)
                return StatusCode(403, new { message = "Forbidden - you can only delete your own events" });

            _context.Events.Remove(e);
            _context.SaveChanges();

            return Ok(new { message = "Event deleted successfully" });
        }
    }
}