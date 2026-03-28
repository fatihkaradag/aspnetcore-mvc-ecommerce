using System.ComponentModel.DataAnnotations;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents a company entity — used for bulk/corporate orders with delayed payment
    public class Company
    {
        // Primary key for the Company entity
        public int Id { get; set; }

        // Company name — required field
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Street address of the company
        [MaxLength(200)]
        public string? StreetAddress { get; set; }

        // City where the company is located
        [MaxLength(100)]
        public string? City { get; set; }

        // State or province where the company is located
        [MaxLength(100)]
        public string? State { get; set; }

        // Postal or ZIP code of the company
        [MaxLength(20)]
        public string? PostalCode { get; set; }

        // Contact phone number for the company
        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
}