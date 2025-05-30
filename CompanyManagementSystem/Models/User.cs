using System;
using System.Collections.Generic;

namespace CompanyManagementSystem.Models
{
    public class User
    {
        public User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PurchaseOrders = new List<PurchaseOrder>();
            Companies = new List<Company>();
        }

        public int Id { get; set; } // Assuming Id is your primary key
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // Add other user properties as needed (e.g., Email, Role)

        // Navigation property for Purchase Orders (if you added UserId to PurchaseOrder)
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public List<Company> Companies { get; set; }
    }
}
