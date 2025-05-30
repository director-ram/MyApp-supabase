using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CompanyManagementSystem.Models
{
    public class Company
    {
        public Company()
        {
            Name = string.Empty;
            Address = string.Empty;
            PurchaseOrders = new List<PurchaseOrder>();
        }

        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public User? User { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}