using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents an order detail entity — stores individual line items for an order
    public class OrderDetail
    {
        // Primary key for the OrderDetails entity
        [Key]
        public int Id { get; set; }

        // Foreign key linking detail to its parent order header
        [Required]
        public int OrderHeaderId { get; set; }

        // Navigation property to the related OrderHeader entity
        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public OrderHeader? OrderHeader { get; set; }

        // Foreign key linking detail to its product
        [Required]
        public int ProductId { get; set; }

        // Navigation property to the related Product entity
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        // Quantity of the product ordered
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Unit price at the time of order — snapshot to preserve historical pricing
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public double Price { get; set; }
    }
}