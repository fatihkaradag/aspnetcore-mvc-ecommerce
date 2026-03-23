using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    // Handles all HTTP requests related to Category management
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Category/Index — retrieves and displays all categories
        public async Task<IActionResult> Index()
        {
            // Fetches all categories asynchronously via repository
            IEnumerable<Category> objCategoryList = await _unitOfWork.Category.GetAllAsync();
            return View(objCategoryList.ToList());
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
                // Adds new category via repository and persists changes
                _unitOfWork.Category.Add(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: /Category/Edit — retrieves category by id and returns edit form
        public async Task<IActionResult> Edit(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches category asynchronously via repository
            Category? categoryFromDb = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            if (categoryFromDb == null) return NotFound();

            return View(categoryFromDb);
        }

        // POST: /Category/Edit — validates and updates existing category in database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category obj)
        {
            // Cross-check route id with form id to prevent manipulation
            if (id != obj.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                // Verify the record actually exists before updating
                Category? existing = await _unitOfWork.Category.GetAsync(u => u.Id == obj.Id);
                if (existing == null) return NotFound();

                // Copy new values into tracked entity to avoid EF Core tracking conflict
                existing.Name = obj.Name;
                existing.DisplayOrder = obj.DisplayOrder;

                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: /Category/Delete — retrieves category by id and returns delete confirmation form
        public async Task<IActionResult> Delete(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches category asynchronously via repository
            Category? categoryFromDb = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            if (categoryFromDb == null) return NotFound();

            return View(categoryFromDb);
        }

        // POST: /Category/Delete — removes the category from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches category asynchronously via repository before removing
            Category? obj = await _unitOfWork.Category.GetAsync(u => u.Id == id);
            if (obj == null) return NotFound();

            // Removes category via repository and persists changes
            _unitOfWork.Category.Remove(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Category deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}