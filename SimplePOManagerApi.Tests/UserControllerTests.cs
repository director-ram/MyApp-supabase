using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Controllers;
using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplePOManagerApi.Tests
{
    public class UserControllerTests
    {
        private AppDbContext _context;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Seed the database with some test data
            _context.Users.Add(new User { Id = 1, FirstName = "Test", LastName = "User", Username = "testuser", Email = "test@example.com", Password = "password" });
            _context.SaveChanges();

            _controller = new UserController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Act
            var result = await _controller.GetUsers();

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<IEnumerable<User>>>(result);
            var actionResult = (result.Result as ObjectResult)?.Value as IEnumerable<User>;
            NUnit.Framework.Assert.AreEqual(1, actionResult?.Count());
        }

        [Test]
        public async Task GetUser_ReturnsCorrectUser()
        {
            // Act
            var result = await _controller.GetUser(1);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<User>>(result);
            var actionResult = (result.Result as ObjectResult)?.Value as User;
            NUnit.Framework.Assert.AreEqual("Test", actionResult?.FirstName);
        }

        [Test]
        public async Task GetUser_ReturnsNotFoundForInvalidId()
        {
            // Act
            var result = await _controller.GetUser(2);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}
