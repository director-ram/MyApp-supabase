using Microsoft.AspNetCore.Mvc;
using CompanyManagementSystem.Controllers;
using CompanyManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CompanyManagementSystem.Tests
{
    public class CompanyControllerTests
    {
        private AppDbContext _context;
        private CompanyController _controller;

        [SetUp]
        public void Setup()
        {
            // Setup an in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed some data for testing
            _context.Companies.AddRange(
                new Company { Id = 1, Name = "Company A", Address = "Address A" },
                new Company { Id = 2, Name = "Company B", Address = "Address B" }
            );
            _context.SaveChanges();

            _controller = new CompanyController(_context);
        }

        [Test]
        public void Get_ReturnsAllCompanies()
        {
            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsInstanceOf<ActionResult<List<Company>>>(result);
            var companies = (result.Result as OkObjectResult)?.Value as List<Company>;
            Assert.IsNotNull(companies);
            Assert.AreEqual(2, companies.Count);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}