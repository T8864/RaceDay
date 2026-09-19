using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return Unauthorized(new { message = "Login required" });

            var user = _context.Users.FirstOrDefault(u => u.UserID == userID);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.Phone,
                user.CreatedAt
            });
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return Unauthorized(new { message = "Login required" });

            var user = _context.Users.FirstOrDefault(u => u.UserID == userID);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;

            _context.SaveChanges();

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}