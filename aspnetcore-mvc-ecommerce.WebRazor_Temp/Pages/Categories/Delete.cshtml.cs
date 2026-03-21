using aspnetcore_mvc_ecommerce.WebRazor_Temp.Data;
using aspnetcore_mvc_ecommerce.WebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace aspnetcore_mvc_ecommerce.WebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = _db.Categories.Find(id);

            }
        }

        public IActionResult OnPost()
        {
            if (Category.Id == null || Category.Id == 0) return NotFound();

            Category? obj = _db.Categories.FirstOrDefault(u => u.Id == Category.Id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(obj);
            _db.SaveChangesAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToPage("Index");
        }
    }
}
