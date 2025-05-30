using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CompanyManagementSystem.Models
{
    public class PurchaseOrder
    {
        public PurchaseOrder()
        {
            LineItems = new List<LineItem>();
        }

        public int Id { get; set; }

        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [Required]
        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }

        [JsonIgnore]
        public Company? Company { get; set; }

        [Required]
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("notificationEmail")]
        public string? NotificationEmail { get; set; }

        [JsonPropertyName("notificationTime")]
        public DateTime? NotificationTime { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";

        [JsonPropertyName("lineItems")]
        public ICollection<LineItem> LineItems { get; set; }

        public void SetNotificationTime(DateTime localTime)
        {
            NotificationTime = localTime.ToUniversalTime();
        }

        public DateTime? GetNotificationTimeLocal()
        {
            return NotificationTime?.ToLocalTime();
        }
    }
}
