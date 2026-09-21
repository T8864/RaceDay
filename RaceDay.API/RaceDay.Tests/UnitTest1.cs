using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
using Microsoft.Extensions.Configuration;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private JwtService GetJwtService()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "RaceDaySuperSecretKey12345678901234" },
                    { "Jwt:Issuer", "RaceDay.API" },
                    { "Jwt:Audience", "RaceDay.Client" }
                }!)
                .Build();
            return new JwtService(config);
        }

        // Test 1: Register with valid data returns 201
        [Fact]
        public void Register_ValidUser_Returns201()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var dto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                Password = "Password123",
                Role = "Organiser",
                Phone = "0821234567"
            };

            var result = controller.Register(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(201, result!.StatusCode);
        }

        // Test 2: Register with duplicate email returns 400
        [Fact]
        public void Register_DuplicateEmail_Returns400()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var dto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                Password = "Password123",
                Role = "Organiser"
            };

            controller.Register(dto);
            var result = controller.Register(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        // Test 3: Register with invalid role returns 400
        [Fact]
        public void Register_InvalidRole_Returns400()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var dto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                Password = "Password123",
                Role = "Admin"
            };

            var result = controller.Register(dto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        // Test 4: Login with valid credentials returns 200 with token
        [Fact]
        public void Login_ValidCredentials_Returns200WithToken()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                Password = "Password123",
                Role = "Organiser"
            };
            controller.Register(registerDto);

            var loginDto = new LoginDto
            {
                Email = "john@test.com",
                Password = "Password123"
            };

            var result = controller.Login(loginDto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        // Test 5: Login with wrong password returns 401
        [Fact]
        public void Login_WrongPassword_Returns401()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john@test.com",
                Password = "Password123",
                Role = "Organiser"
            };
            controller.Register(registerDto);

            var loginDto = new LoginDto
            {
                Email = "john@test.com",
                Password = "WrongPassword"
            };

            var result = controller.Login(loginDto) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result!.StatusCode);
        }

        // Test 6: Login with non existent email returns 401
        [Fact]
        public void Login_NonExistentEmail_Returns401()
        {
            var context = GetInMemoryContext();
            var controller = new AuthController(context, GetJwtService());

            var loginDto = new LoginDto
            {
                Email = "nobody@test.com",
                Password = "Password123"
            };

            var result = controller.Login(loginDto) as UnauthorizedObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result!.StatusCode);
        }
    }
}
