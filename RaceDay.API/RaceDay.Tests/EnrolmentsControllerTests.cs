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
    public class EnrolmentsControllerTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private EnrolmentsController GetControllerWithUser(AppDbContext context, int userId, string role)
        {
            var controller = new EnrolmentsController(context);
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
            context.Users.AddRange(
                new User { UserID = 1, FirstName = "John", LastName = "Smith", Email = "john@test.com", PasswordHash = "hash", Role = "Organiser" },
                new User { UserID = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@test.com", PasswordHash = "hash", Role = "Participant" }
            );

            context.Categories.Add(new Category { CategoryID = 1, CategoryName = "5KM Fun Run", Description = "Beginner run" });

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

        // Test 1: Participant can enrol in an event
        [Fact]
        public void Enrol_ValidParticipant_Returns201()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 2, "Participant");

            var dto = new CreateEnrolmentDto
            {
                EventID = 1,
                CategoryID = 1
            };

            var result = controller.Enrol(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
        }

        // Test 2: Participant cannot enrol twice in same event
        [Fact]
        public void Enrol_DuplicateEnrolment_Returns409()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 2, "Participant");

            var dto = new CreateEnrolmentDto { EventID = 1, CategoryID = 1 };

            controller.Enrol(dto);
            var result = controller.Enrol(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(409, result!.StatusCode);
        }

        // Test 3: Organiser can view all enrolments
        [Fact]
        public void GetAll_AsOrganiser_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 1, "Organiser");

            var result = controller.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 4: Participant can view their own enrolments
        [Fact]
        public void GetMyEnrolments_AsParticipant_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 2, "Participant");

            var result = controller.GetMyEnrolments() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 5: Participant can cancel their own enrolment
        [Fact]
        public void Cancel_OwnEnrolment_ReturnsOk()
        {
            var context = GetInMemoryContext();
            SeedData(context);
            var controller = GetControllerWithUser(context, 2, "Participant");

            var dto = new CreateEnrolmentDto { EventID = 1, CategoryID = 1 };
            controller.Enrol(dto);

            var enrolment = context.Enrolments.First();
            var result = controller.Cancel(enrolment.EnrolmentID) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 6: Enrolment fails when event is full
        [Fact]
        public void Enrol_FullEvent_Returns400()
        {
            var context = GetInMemoryContext();
            SeedData(context);

            var fullEvent = context.Events.First();
            fullEvent.MaxParticipants = 0;
            context.SaveChanges();

            var controller = GetControllerWithUser(context, 2, "Participant");
            var dto = new CreateEnrolmentDto { EventID = 1, CategoryID = 1 };

            var result = controller.Enrol(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }
    }
}