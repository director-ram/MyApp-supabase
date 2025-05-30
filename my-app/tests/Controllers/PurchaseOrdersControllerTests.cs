using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CompanyManagementSystem.Controllers;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Data;

namespace CompanyManagementSystem.Tests.Controllers
{
    public class PurchaseOrdersControllerTests
    {
        private readonly AppDbContext _context;
        private readonly PurchaseOrdersController _controller;

        public PurchaseOrdersControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _controller = new PurchaseOrdersController(_context);
        }

        [Fact]
        public async Task Get_ReturnsListOfPurchaseOrders()
        {
            // Arrange
            var userId = 1;
            _context.PurchaseOrders.Add(new PurchaseOrder { UserId = userId, OrderDate = DateTime.Now });
            _context.SaveChanges();

            // Act
            var result = await _controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<PurchaseOrder>>>(result);
            var purchaseOrders = Assert.IsType<List<PurchaseOrder>>(actionResult.Value);
            Assert.Single(purchaseOrders);
        }

        [Fact]
        public async Task Post_CreatesPurchaseOrder()
        {
            // Arrange
            var userId = 1;
            var purchaseOrderDto = new PurchaseOrderDto
            {
                CompanyId = 1,
                OrderDate = DateTime.Now,
                TotalAmount = 100,
                NotificationEmail = "test@example.com",
                LineItems = new List<LineItemDto>
                {
                    new LineItemDto { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                }
            };

            // Act
            var result = await _controller.Post(purchaseOrderDto);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(1, _context.PurchaseOrders.Count());
        }

        [Fact]
        public async Task Delete_RemovesPurchaseOrder()
        {
            // Arrange
            var userId = 1;
            var purchaseOrder = new PurchaseOrder { UserId = userId };
            _context.PurchaseOrders.Add(purchaseOrder);
            _context.SaveChanges();

            // Act
            var result = await _controller.Delete(purchaseOrder.Id);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Empty(_context.PurchaseOrders);
        }
    }
}