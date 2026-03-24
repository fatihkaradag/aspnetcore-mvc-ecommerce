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
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = webHostEnvironment;
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
                string wwwRootPath = _hostEnvironment.WebRootPath;

                // Handles image upload if a new file is provided
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, "images", "products");
                    var extension = Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploads, fileName + extension);

                    // If the uploads directory doesn't exist, create it
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Delete old image if one already exists — runs for both create and update
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image file to wwwroot/images/products
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (productVM.Product.Id == 0)
                {
                    // Create mode — add new product to database
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    // Edit mode — update existing product in database
                    _unitOfWork.Product.Update(productVM.Product);
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