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
    public class PurchaseOrderControllerTests
    {
        private AppDbContext _context;
        private PurchaseOrderController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Seed the database with some test data
            var company = new Company { Id = 1, Name = "Test Company", Address = "Test Address" };
            _context.Companies.Add(company);

            var purchaseOrder1 = new PurchaseOrder { Id = 1, OrderNumber = "PO1", OrderDate = DateTime.Now, CompanyId = 1, Company = company, LineItems = new List<LineItem>() };
            var purchaseOrder2 = new PurchaseOrder { Id = 2, OrderNumber = "PO2", OrderDate = DateTime.Now, CompanyId = 1, Company = company, LineItems = new List<LineItem>() };
            _context.PurchaseOrders.AddRange(purchaseOrder1, purchaseOrder2);

            _context.SaveChanges();

            _controller = new PurchaseOrderController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetPurchaseOrders_ReturnsAllPurchaseOrders()
        {
            // Act
            var result = await _controller.GetPurchaseOrders();

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<IEnumerable<PurchaseOrder>>>(result);
            var purchaseOrders = (result.Result as ObjectResult)?.Value as List<PurchaseOrder>;
            NUnit.Framework.Assert.AreEqual(2, purchaseOrders?.Count);
            NUnit.Framework.Assert.AreEqual("PO1", purchaseOrders?[0].OrderNumber);
            NUnit.Framework.Assert.AreEqual("PO2", purchaseOrders?[1].OrderNumber);
        }

        [Test]
        public async Task GetPurchaseOrder_ReturnsCorrectPurchaseOrder()
        {
            // Act
            var result = await _controller.GetPurchaseOrder(1);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ActionResult<PurchaseOrder>>(result);
            var purchaseOrder = (result.Result as ObjectResult)?.Value as PurchaseOrder;
            NUnit.Framework.Assert.AreEqual("PO1", purchaseOrder?.OrderNumber);
        }

        [Test]
        public async Task GetPurchaseOrder_ReturnsNotFoundForInvalidId()
        {
            // Act
            var result = await _controller.GetPurchaseOrder(3);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostPurchaseOrder_CreatesNewPurchaseOrder()
        {
            // Arrange
            var newPurchaseOrder = new PurchaseOrder { OrderNumber = "PO3", OrderDate = DateTime.Now, CompanyId = 1, Company = new Company { Id = 1 }, LineItems = new List<LineItem>() };

            // Act
            var result = await _controller.PostPurchaseOrder(newPurchaseOrder);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdPurchaseOrder = (result.Result as CreatedAtActionResult)?.Value as PurchaseOrder;
            NUnit.Framework.Assert.AreEqual("PO3", createdPurchaseOrder?.OrderNumber);

            NUnit.Framework.Assert.AreEqual(3, _context.PurchaseOrders.Count());
        }

        [Test]
        public async Task DeletePurchaseOrder_DeletesExistingPurchaseOrder()
        {
            // Act
            var result = await _controller.DeletePurchaseOrder(1);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<NoContentResult>(result);
            NUnit.Framework.Assert.AreEqual(1, _context.PurchaseOrders.Count());
        }

        [Test]
        public async Task DeletePurchaseOrder_ReturnsNotFoundForInvalidId()
        {
            // Act
            var result = await _controller.DeletePurchaseOrder(3);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}