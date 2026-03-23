using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using aspnetcore_mvc_ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    // Handles all HTTP requests related to Product management
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /Product/Index — retrieves and displays all products
        public async Task<IActionResult> Index()
        {
            // Fetches all products asynchronously via repository including category info
            IEnumerable<Product> objProductList = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
            return View(objProductList.ToList());
        }

        // GET: /Product/Upsert — returns create form if id is null, edit form if id exists
        public async Task<IActionResult> Upsert(int? id)
        {
            // Builds the ViewModel with category dropdown
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = (await _unitOfWork.Category.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
            };

            if (id == null || id == 0)
            {
                // Create mode — return empty product form
                return View(productVM);
            }
            else
            {
                // Edit mode — fetch existing product and populate form
                Product? productFromDb = await _unitOfWork.Product.GetAsync(u => u.Id == id);
                if (productFromDb == null) return NotFound();

                productVM.Product = productFromDb;
                return View(productVM);
            }
        }

        // POST: /Product/Upsert — creates or updates product based on Id value
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    // Create mode — add new product to database
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    // Edit mode — fetch and update existing product
                    Product? existing = await _unitOfWork.Product.GetAsync(u => u.Id == productVM.Product.Id);
                    if (existing == null) return NotFound();

                    // Copy new values into tracked entity to avoid EF Core tracking conflict
                    existing.Title = productVM.Product.Title;
                    existing.Description = productVM.Product.Description;
                    existing.Author = productVM.Product.Author;
                    existing.ISBN = productVM.Product.ISBN;
                    existing.ListPrice = productVM.Product.ListPrice;
                    existing.Price = productVM.Product.Price;
                    existing.Price50 = productVM.Product.Price50;
                    existing.Price100 = productVM.Product.Price100;
                    existing.CategoryId = productVM.Product.CategoryId;

                    TempData["success"] = "Product updated successfully.";
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }

            // Repopulates category dropdown if validation fails
            productVM.CategoryList = (await _unitOfWork.Category.GetAllAsync())
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            return View(productVM);
        }

        // GET: /Product/Delete — retrieves product by id and returns delete confirmation form
        public async Task<IActionResult> Delete(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches product with category info asynchronously via repository
            Product? productFromDb = await _unitOfWork.Product.GetAsync(u => u.Id == id, includeProperties: "Category");
            if (productFromDb == null) return NotFound();

            return View(productFromDb);
        }

        // POST: /Product/Delete — removes the product from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches product asynchronously via repository before removing
            Product? obj = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (obj == null) return NotFound();

            // Removes product via repository and persists changes
            _unitOfWork.Product.Remove(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Product deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}