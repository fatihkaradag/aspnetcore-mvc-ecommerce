using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnetcore_mvc_ecommerce.Models
{
    // Represents a product category entity mapped to the database
    public class Category
    {
        // Primary key for the Category entity
        [Key]
        public int Id { get; set; }

        // Category name — required field, cannot be null or empty
        [Required]
        [MaxLength(30)]
        [DisplayName("Name")]
        public string Name { get; set; }
        

        // Determines the display order of categories in listings
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }

        

    }
}