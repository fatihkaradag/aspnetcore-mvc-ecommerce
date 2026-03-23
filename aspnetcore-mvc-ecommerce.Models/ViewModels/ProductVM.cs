using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace aspnetcore_mvc_ecommerce.Models.ViewModels
{
    // ViewModel for Product create and edit forms — combines product data with category dropdown
    public class ProductVM
    {
        // Product entity to be created or updated
        public Product Product { get; set; } = new();

        // Category dropdown list for the select input
        [ValidateNever] // Skip validation for this property as it's only used for display
        public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
    }
}