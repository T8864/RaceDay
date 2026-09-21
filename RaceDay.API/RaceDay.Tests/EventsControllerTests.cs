using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;
using System.Security.Claims;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private EventsController GetControllerWithUser(AppDbContext context, int userId, string role)
        {
            var controller = new EventsController(context);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            return controller;
        }

        private void SeedData(AppDbContext context)
        {
            context.Users.Add(new User
            {
                UserID = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                PasswordHash = "hash",
                Role = "Organiser"
            });

            context.Categories.Add(new Category
            {
                CategoryID = 1,
                CategoryName = "5KM Fun Run",
                Description = "Beginner run"
            });

            context.Events.Add(new Event
            {
                EventID = 1,
                EventName = "Test Race",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Cape Town",
                Distance = "5km",
                Status = "Upcoming",
                MaxParticipants = 100,
                OrganiserID = 1,
                CategoryID = 1
            });

            context.SaveChanges();
        }

        // Test 1: Get all events returns 200
        [Fact]
        public void GetAll_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var result = controller.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 2: Get event by valid ID returns 200
        [Fact]
        public void GetById_ValidId_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var result = controller.GetById(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 3: Get event by invalid ID returns 404
        [Fact]
        public void GetById_InvalidId_Returns404()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var result = controller.GetById(999) as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result!.StatusCode);
        }

        // Test 4: Organiser can create event
        [Fact]
        public void Create_AsOrganiser_Returns201()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var dto = new CreateEventDto
            {
                EventName = "New Race",
                EventDate = DateTime.Now.AddDays(60),
                Location = "Johannesburg",
                Distance = "10km",
                MaxParticipants = 200,
                CategoryID = 1
            };

            var result = controller.Create(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
        }

        // Test 5: Organiser can delete their own event
        [Fact]
        public void Delete_OwnEvent_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var result = controller.Delete(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 6: Organiser cannot delete another organisers event
        [Fact]
        public void Delete_OtherOrganiserEvent_Returns403()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 2, "Organiser");

            var result = controller.Delete(1) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(403, result!.StatusCode);
        }
    }
}