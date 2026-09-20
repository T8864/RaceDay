using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
            var userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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