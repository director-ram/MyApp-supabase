using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CompanyManagementSystem.Models
{
    public class LineItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [JsonIgnore]
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [Required]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}