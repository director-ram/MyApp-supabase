using Microsoft.AspNetCore.Mvc;
using CompanyManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CompanyManagementSystem.Services;
using Npgsql;

namespace CompanyManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PurchaseOrdersController> _logger;
        private readonly IEmailService _emailService;

        public PurchaseOrdersController(
            AppDbContext context,
            ILogger<PurchaseOrdersController> logger,
            IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from claims
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim.Value);
        }

        /// <summary>
        /// Gets all purchase orders for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
        {
            try
            {
                var purchaseOrders = await _context.PurchaseOrders
                    .Include(po => po.Company)
                    .Include(po => po.LineItems)
                    .Select(po => new
                    {
                        po.Id,
                        po.CompanyId,
                        Company = po.Company != null ? new { po.Company.Id, po.Company.Name } : null,
                        po.OrderDate,
                        po.TotalAmount,
                        po.Status,
                        po.NotificationEmail,
                        po.NotificationTime,
                        LineItems = po.LineItems.Select(li => new
                        {
                            li.Id,
                            li.ProductId,
                            li.Quantity,
                            li.UnitPrice
                        })
                    })
                    .ToListAsync();

                return Ok(purchaseOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase orders");
                return StatusCode(500, "An error occurred while retrieving purchase orders");
            }
        }

        /// <summary>
        /// Gets a specific purchase order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            try
            {
                var purchaseOrder = await _context.PurchaseOrders
                    .Include(po => po.Company)
                    .Include(po => po.LineItems)
                    .FirstOrDefaultAsync(po => po.Id == id);

                if (purchaseOrder == null)
                {
                    return NotFound();
                }

                // Load company details if needed
                if (purchaseOrder.Company == null)
                {
                    var company = await _context.Companies
                        .Where(c => c.Id == purchaseOrder.CompanyId)
                        .Select(c => new Company
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Address = c.Address,
                            UserId = c.UserId
                        })
                        .FirstOrDefaultAsync();
                        
                    purchaseOrder.Company = company;
                }

                return Ok(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase order {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the purchase order");
            }
        }

        /// <summary>
        /// Creates a new purchase order
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            try
            {
                _logger.LogInformation("Creating purchase order: {@PurchaseOrder}", purchaseOrder);

                // Set the UserId from the authenticated user
                purchaseOrder.UserId = GetCurrentUserId();
                _logger.LogInformation("Set UserId to {UserId}", purchaseOrder.UserId);

                // Validate required fields
                if (purchaseOrder.CompanyId <= 0)
                {
                    _logger.LogWarning("Invalid CompanyId: {CompanyId}", purchaseOrder.CompanyId);
                    return BadRequest("CompanyId is required and must be greater than 0");
                }

                // Verify company exists
                var company = await _context.Companies.FindAsync(purchaseOrder.CompanyId);
                if (company == null)
                {
                    _logger.LogWarning("Company not found: {CompanyId}", purchaseOrder.CompanyId);
                    return BadRequest("Invalid company selected");
                }

                if (purchaseOrder.OrderDate == default)
                {
                    _logger.LogWarning("Invalid OrderDate: {OrderDate}", purchaseOrder.OrderDate);
                    return BadRequest("OrderDate is required");
                }

                // Ensure OrderDate is in UTC
                purchaseOrder.OrderDate = purchaseOrder.OrderDate.ToUniversalTime();
                _logger.LogInformation("Converted OrderDate to UTC: {OrderDate}", purchaseOrder.OrderDate);

                // Set notification time if email is provided
                if (!string.IsNullOrEmpty(purchaseOrder.NotificationEmail))
                {
                    // Set notification time to 1 hour before order date
                    purchaseOrder.NotificationTime = purchaseOrder.OrderDate.AddHours(-1).ToUniversalTime();
                    _logger.LogInformation("Set notification time to {NotificationTime}", purchaseOrder.NotificationTime);
                }

                // Initialize LineItems collection if null
                if (purchaseOrder.LineItems == null)
                {
                    purchaseOrder.LineItems = new List<LineItem>();
                    _logger.LogInformation("Initialized empty LineItems collection");
                }

                // Validate line items
                if (!purchaseOrder.LineItems.Any())
                {
                    _logger.LogWarning("No line items provided");
                    return BadRequest("At least one line item is required");
                }

                // Create a new list for validated line items
                var validatedLineItems = new List<LineItem>();

                // Validate and process line items
                foreach (var item in purchaseOrder.LineItems)
                {
                    if (item.ProductId <= 0)
                    {
                        _logger.LogWarning("Invalid ProductId in line item: {ProductId}", item.ProductId);
                        return BadRequest("ProductId must be greater than 0");
                    }
                    if (item.Quantity <= 0)
                    {
                        _logger.LogWarning("Invalid Quantity in line item: {Quantity}", item.Quantity);
                        return BadRequest("Quantity must be greater than 0");
                    }
                    if (item.UnitPrice <= 0)
                    {
                        _logger.LogWarning("Invalid UnitPrice in line item: {UnitPrice}", item.UnitPrice);
                        return BadRequest("UnitPrice must be greater than 0");
                    }

                    // Create a new line item
                    var validatedItem = new LineItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    validatedLineItems.Add(validatedItem);
                }

                // Calculate total amount
                purchaseOrder.TotalAmount = validatedLineItems.Sum(item => item.Quantity * item.UnitPrice);
                _logger.LogInformation("Calculated total amount: {TotalAmount}", purchaseOrder.TotalAmount);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Create the purchase order first
                    var newPurchaseOrder = new PurchaseOrder
                    {
                        UserId = purchaseOrder.UserId,
                        CompanyId = purchaseOrder.CompanyId,
                        OrderDate = purchaseOrder.OrderDate,
                        TotalAmount = purchaseOrder.TotalAmount,
                        NotificationEmail = purchaseOrder.NotificationEmail,
                        NotificationTime = purchaseOrder.NotificationTime,
                        Status = "Pending"
                    };

                    _context.PurchaseOrders.Add(newPurchaseOrder);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully created purchase order with ID {Id}", newPurchaseOrder.Id);

                    // Add line items with the correct PurchaseOrderId
                    foreach (var item in validatedLineItems)
                    {
                        item.PurchaseOrderId = newPurchaseOrder.Id;
                        _context.LineItems.Add(item);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully added {Count} line items", validatedLineItems.Count);

                    await transaction.CommitAsync();
                    _logger.LogInformation("Transaction committed successfully");

                    // Send immediate notifications
                    if (!string.IsNullOrEmpty(newPurchaseOrder.NotificationEmail))
                    {
                        try
                        {
                            await _emailService.SendEmailAsync(
                                newPurchaseOrder.NotificationEmail,
                                $"Purchase Order Created: PO-{newPurchaseOrder.Id}",
                                $"Your purchase order has been created successfully.\nOrder Date: {newPurchaseOrder.OrderDate.ToLocalTime():g}\nTotal Amount: ${newPurchaseOrder.TotalAmount:N2}"
                            );
                            _logger.LogInformation("Email notification sent successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send email notification for PO {Id}", newPurchaseOrder.Id);
                        }
                    }

                    // Return a simplified response
                    var response = new
                    {
                        id = newPurchaseOrder.Id,
                        companyId = newPurchaseOrder.CompanyId,
                        orderDate = newPurchaseOrder.OrderDate,
                        totalAmount = newPurchaseOrder.TotalAmount,
                        notificationEmail = newPurchaseOrder.NotificationEmail,
                        notificationTime = newPurchaseOrder.NotificationTime,
                        status = newPurchaseOrder.Status,
                        lineItems = validatedLineItems.Select(item => new
                        {
                            id = item.Id,
                            productId = item.ProductId,
                            quantity = item.Quantity,
                            unitPrice = item.UnitPrice
                        }).ToList()
                    };

                    return CreatedAtAction(nameof(GetPurchaseOrder), new { id = newPurchaseOrder.Id }, response);
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(dbEx, "Database error while creating purchase order: {Message}\nInner Exception: {InnerMessage}", 
                        dbEx.Message, dbEx.InnerException?.Message);
                    return StatusCode(500, new { 
                        message = "Database error while creating purchase order", 
                        error = dbEx.Message,
                        innerError = dbEx.InnerException?.Message
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during transaction while creating purchase order: {Message}", ex.Message);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order: {Message}", ex.Message);
                return StatusCode(500, new { 
                    message = "An error occurred while creating the purchase order", 
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Updates a purchase order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return BadRequest();
            }

            try
            {
                // Ensure OrderDate is in UTC
                purchaseOrder.OrderDate = purchaseOrder.OrderDate.ToUniversalTime();

                // If notification time is set, convert it to UTC
                if (purchaseOrder.NotificationTime.HasValue)
                {
                    purchaseOrder.NotificationTime = purchaseOrder.NotificationTime.Value.ToUniversalTime();
                }

                _context.Entry(purchaseOrder).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order {Id}", id);
                return StatusCode(500, "An error occurred while updating the purchase order");
            }
        }

        /// <summary>
        /// Deletes a purchase order by ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete purchase order {Id}", id);

                // First check if the purchase order exists
                var purchaseOrder = await _context.PurchaseOrders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(po => po.Id == id);

                if (purchaseOrder == null)
                {
                    _logger.LogWarning("Purchase order {Id} not found", id);
                    return NotFound(new { message = "Purchase order not found" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Delete everything in a single SQL command using CASCADE
                    _logger.LogInformation("Deleting purchase order {Id} and related data", id);

                    // First, delete line items
                    var lineItemsDeleted = await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM \"LineItems\" WHERE \"PurchaseOrderId\" = {0}", id);
                    _logger.LogInformation("Deleted {Count} line items", lineItemsDeleted);

                    // Then delete the purchase order
                    var purchaseOrderDeleted = await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM \"PurchaseOrders\" WHERE \"Id\" = {0}", id);

                    if (purchaseOrderDeleted == 0)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning("No purchase order was deleted for ID {Id}", id);
                        return NotFound(new { message = "Purchase order not found" });
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully deleted purchase order {Id} and related data", id);

                    return NoContent();
                }
                catch (PostgresException pgEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(pgEx, "PostgreSQL error while deleting purchase order {Id}: {Message}\nDetail: {Detail}", 
                        id, pgEx.Message, pgEx.Detail);
                    return StatusCode(500, new { 
                        message = "Database error while deleting purchase order", 
                        error = pgEx.Message,
                        detail = pgEx.Detail
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during transaction while deleting purchase order {Id}: {Message}\nStack Trace: {StackTrace}", 
                        id, ex.Message, ex.StackTrace);
                    return StatusCode(500, new { 
                        message = "An error occurred while deleting the purchase order", 
                        error = ex.Message,
                        stackTrace = ex.StackTrace
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase order {Id}: {Message}\nStack Trace: {StackTrace}", 
                    id, ex.Message, ex.StackTrace);
                return StatusCode(500, new { 
                    message = "An error occurred while deleting the purchase order", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }

        public class PurchaseOrderDto
        {
            public int? CompanyId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string? NotificationEmail { get; set; }
            public required List<LineItemDto> LineItems { get; set; }
        }

        public class LineItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                _logger.LogInformation("Testing email sending to {Email}", request.Email);
                await _emailService.SendEmailAsync(
                    request.Email,
                    "Test Email from Purchase Order System",
                    "This is a test email to verify that the email notification system is working correctly."
                );
                return Ok(new { message = "Test email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email");
                return StatusCode(500, new { message = "Failed to send test email", error = ex.Message });
            }
        }

        public class TestEmailRequest
        {
            public string Email { get; set; } = string.Empty;
        }
    }
}