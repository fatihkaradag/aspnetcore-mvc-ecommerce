using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents a product entity mapped to the database
    public class Product
    {
        // Primary key for the Product entity
        [Key]
        public int Id { get; set; }

        // Product title — required, max 30 characters
        [Required]
        [MaxLength(30)]
        [DisplayName("Title")]
        public string Title { get; set; } = string.Empty;

        // Product description — required field
        [Required]
        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        // Author of the book — required field
        [Required]
        [MaxLength(100)]
        [DisplayName("Author")]
        public string Author { get; set; } = string.Empty;

        // International Standard Book Number — unique identifier for books
        [Required]
        [MaxLength(20)]
        [DisplayName("ISBN")]
        public string ISBN { get; set; } = string.Empty;

        // Original list price of the product
        [Required]
        [DisplayName("List Price")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1-10000")]
        public double ListPrice { get; set; }

        // Price for orders of 1-50 units
        [Required]
        [DisplayName("Price for 1-50")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1-10000")]
        public double Price { get; set; }

        // Discounted price for orders of 50+ units
        [Required]
        [DisplayName("Price for 50+")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1-10000")]
        public double Price50 { get; set; }

        // Discounted price for orders of 100+ units
        [Required]
        [DisplayName("Price for 100+")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1-10000")]
        public double Price100 { get; set; }

        // Foreign key linking product to its category
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        // Navigation property to the related Category entity
        [ForeignKey("CategoryId")]
        [ValidateNever] // Skip validation for this property as it's handled by CategoryId
        public Category? Category { get; set; }

        // URL of the product image — nullable field
        [ValidateNever] // Skip validation for this property as it's optional
        public string? ImageUrl { get; set; } = string.Empty;
    }
}