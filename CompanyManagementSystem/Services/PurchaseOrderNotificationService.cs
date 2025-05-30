using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CompanyManagementSystem.Services
{
    public class PurchaseOrderNotificationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PurchaseOrderNotificationService> _logger;

        public PurchaseOrderNotificationService(
            IServiceProvider services,
            ILogger<PurchaseOrderNotificationService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Purchase Order Notification Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for purchase order notifications");
                    await CheckAndSendNotifications();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking purchase order notifications");
                }

                // Check every 30 seconds instead of every minute
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckAndSendNotifications()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now = DateTime.UtcNow;
            _logger.LogInformation("Checking for notifications due at {Now}", now);

            var purchaseOrders = await context.PurchaseOrders
                .Include(po => po.Company)
                .Where(po => po.NotificationTime != null && 
                            po.NotificationTime <= now && 
                            !string.IsNullOrEmpty(po.NotificationEmail))
                .ToListAsync();

            // Filter in memory for DateTimeKind.Utc if needed
            purchaseOrders = purchaseOrders
                .Where(po => po.NotificationTime.HasValue && po.NotificationTime.Value.Kind == DateTimeKind.Utc)
                .ToList();

            _logger.LogInformation("Found {Count} purchase orders with pending notifications", purchaseOrders.Count);

            foreach (var po in purchaseOrders)
            {
                try
                {
                    if (string.IsNullOrEmpty(po.NotificationEmail))
                    {
                        _logger.LogWarning("Purchase order {Id} has no notification email", po.Id);
                        continue;
                    }

                    var subject = $"Purchase Order Reminder: {po.Id}";
                    var body = $@"
                        <h3>Purchase Order Reminder</h3>
                        <p>This is a reminder that you have a purchase order scheduled for {po.OrderDate.ToLocalTime():g}.</p>
                        <p><strong>Details:</strong></p>
                        <ul>
                            <li>Order ID: {po.Id}</li>
                            <li>Total Amount: ${po.TotalAmount:N2}</li>
                            <li>Company: {po.Company?.Name ?? "N/A"}</li>
                            <li>Order Date: {po.OrderDate.ToLocalTime():g}</li>
                        </ul>
                        <p>Please review this purchase order and take necessary actions.</p>";

                    _logger.LogInformation("Sending notification for purchase order {Id} to {Email}", po.Id, po.NotificationEmail);
                    await emailService.SendEmailAsync(po.NotificationEmail, subject, body);
                    
                    po.NotificationTime = null; // Clear the notification time after sending
                    _logger.LogInformation("Cleared notification time for purchase order {Id}", po.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notification for purchase order {Id}", po.Id);
                }
            }

            if (purchaseOrders.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} purchase orders after sending notifications", purchaseOrders.Count);
            }
        }
    }
}