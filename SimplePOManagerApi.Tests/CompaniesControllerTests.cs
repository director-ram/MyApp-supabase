using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Controllers;
using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplePOManagerApi.Tests
{
    public class CompaniesControllerTests
    {
        private AppDbContext _context;
        private CompaniesController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Seed the database with some test data
            _context.Companies.Add(new Company { Id = 1, Name = "Test Company", Address = "Test Address" });
            _context.SaveChanges();

            _controller = new CompaniesController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetCompanies_ReturnsAllCompanies()
        {
            // Act
            var result = await _controller.GetCompanies();

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<IEnumerable<Company>>>(result);
            var actionResult = (result.Result as ObjectResult)?.Value as IEnumerable<Company>;
            NUnit.Framework.Assert.AreEqual(1, actionResult?.Count());
        }

        [Test]
        public async Task GetCompany_ReturnsCorrectCompany()
        {
            // Act
            var result = await _controller.GetCompany(1);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<Company>>(result);
            var actionResult = (result.Result as ObjectResult)?.Value as Company;
            NUnit.Framework.Assert.AreEqual("Test Company", actionResult?.Name);
        }

        [Test]
        public async Task GetCompany_ReturnsNotFoundForInvalidId()
        {
            // Act
            var result = await _controller.GetCompany(2);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}
