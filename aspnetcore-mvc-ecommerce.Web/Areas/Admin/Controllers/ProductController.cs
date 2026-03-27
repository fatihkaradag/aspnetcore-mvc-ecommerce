using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using aspnetcore_mvc_ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    // Handles all HTTP requests related to Product management
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users only
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: /Product/Index — retrieves and displays all products
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetches all products asynchronously including category navigation property
                IEnumerable<Product> objProductList = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
                return View(objProductList.ToList());
            }
            catch (Exception ex)
            {
                // Logs error and returns 500 if product list cannot be fetched
                _logger.LogError(ex, "Error occurred while fetching products for Index.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: /Product/Upsert — returns create form if id is null, edit form if id exists
        public async Task<IActionResult> Upsert(int? id)
        {
            // Builds the ViewModel with empty product and category dropdown
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

                    // Creates upload directory if it does not exist
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Deletes old image file if one already exists
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Saves new image file asynchronously to wwwroot/images/products
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (productVM.Product.Id == 0)
                {
                    // Create mode — adds new product to database
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    // Edit mode — updates existing product in database
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

        // POST: /Product/Delete — removes the product and its image from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches product asynchronously via repository before removing
            Product? obj = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (obj == null) return NotFound();

            // Deletes the physical image file if it exists
            if (!string.IsNullOrEmpty(obj.ImageUrl))
            {
                string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Removes product via repository and persists changes
            _unitOfWork.Product.Remove(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Product deleted successfully.";

            return RedirectToAction("Index");
        }

        #region API CALLS

        // GET: /Product/GetAll — returns all products as JSON for DataTable API calls
        public async Task<IActionResult> GetAll()
        {
            // Retrieves all products with category info and returns as JSON
            IEnumerable<Product> objProductList = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
            return Json(new { data = objProductList });
        }

        // DELETE: /Product/DeleteAjax — removes product and image via AJAX call
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int? id)
        {
            // Fetches product before deletion to access its image path
            var productToBeDeleted = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Deletes the physical image file if it exists
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
            }

            // Removes product and persists changes
            _unitOfWork.Product.Remove(productToBeDeleted);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}