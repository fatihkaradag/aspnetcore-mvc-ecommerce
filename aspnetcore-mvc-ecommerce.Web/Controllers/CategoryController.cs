using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_mvc_ecommerce.Web.Controllers
{
    // Handles all HTTP requests related to Category management
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext db, ILogger<CategoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /Category/Index — retrieves and displays all categories
        public async Task<IActionResult> Index()
        {
            List<Category> objCategoryList = await _db.Categories.ToListAsync();
            return View(objCategoryList);
        }

        // GET: /Category/Create — returns the empty create form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create — validates and saves new category to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                await _db.SaveChangesAsync();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: /Category/Edit — retrieves category by id and returns edit form
        public async Task<IActionResult> Edit(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Fetches category from database using async LINQ query
            Category? categoryFromDb = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        // POST: /Category/Edit — validates and updates existing category in database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category obj)
        {
            if (id != obj.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await _db.Categories.FirstOrDefaultAsync(u => u.Id == obj.Id);
                if (existing == null) return NotFound();

                existing.Name = obj.Name;
                existing.DisplayOrder = obj.DisplayOrder;

                await _db.SaveChangesAsync();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: /Category/Delete — retrieves category by id and returns delete confirmation form
        public async Task<IActionResult> Delete(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Fetches category from database using async LINQ query
            Category? categoryFromDb = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        // POST: /Category/Delete — removes the category from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            // Validates that id is not null
            if (id == null || id == 0) return NotFound();

            // Fetches category from database before removing
            Category? obj = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            // Removes the category and saves changes asynchronously
            _db.Categories.Remove(obj);
            await _db.SaveChangesAsync();
            TempData["success"] = "Category deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}