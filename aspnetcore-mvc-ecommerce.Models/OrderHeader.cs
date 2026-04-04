using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents an order header entity — stores order and payment details
    public class OrderHeader
    {
        // Primary key for the OrderHeader entity
        [Key]
        public int Id { get; set; }

        // Foreign key linking order to the application user who placed it
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        // Navigation property to the related ApplicationUser entity
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

        // Date and time the order was placed
        public DateTime OrderDate { get; set; }

        // Date and time the order was shipped
        public DateTime ShippingDate { get; set; }

        // Total price of the order
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Order total must be a positive value")]
        public double OrderTotal { get; set; }

        // Current status of the order — e.g. Pending, Approved, Shipped
        public string? OrderStatus { get; set; }

        // Current payment status — e.g. Pending, Approved, Rejected
        public string? PaymentStatus { get; set; }

        // Carrier tracking number for shipment
        [MaxLength(100)]
        public string? TrackingNumber { get; set; }

        // Shipping carrier name — e.g. FedEx, UPS
        [MaxLength(100)]
        public string? Carrier { get; set; }

        // Date and time the payment was completed
        public DateTime PaymentDate { get; set; }

        // Due date for delayed payment — used for company accounts
        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }

        // Stripe payment intent ID for payment processing
        [MaxLength(200)]
        public string? PaymentIntentId { get; set; }

        // Shipping address fields — required for order fulfillment
        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        // Full name of the recipient
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}