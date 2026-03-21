using aspnetcore_mvc_ecommerce.WebRazor_Temp.Data;
using aspnetcore_mvc_ecommerce.WebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetcore_mvc_ecommerce.WebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if(id != null && id != 0)
            {
                Category = _db.Categories.Find(id);

            }
        }

        public IActionResult OnPost()
        {
            if (Category.Id != Category.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing =  _db.Categories.Find(Category.Id);
                if (existing == null) return NotFound();

                existing.Name = Category.Name;
                existing.DisplayOrder = Category.DisplayOrder;

                _db.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
