using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents a shopping cart item linking a user to a product with a quantity
    public class ShoppingCart
    {
        // Primary key for the ShoppingCart entity
        [Key]
        public int Id { get; set; }

        // Foreign key linking cart item to its product
        [Required]
        public int ProductId { get; set; }

        // Navigation property to the related Product entity
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        // Quantity of the product added to the cart
        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }

        // Foreign key linking cart item to the application user
        [Required]
        [ValidateNever]
        public string ApplicationUserId { get; set; } = string.Empty;

        // Navigation property to the related ApplicationUser entity
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        // Computed price based on quantity — not mapped to database
        [NotMapped]
        public double Price { get; set; }
    }
}