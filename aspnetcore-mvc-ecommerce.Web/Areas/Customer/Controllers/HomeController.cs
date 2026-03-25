using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        // Action Name Fix: If your link uses 'productId', rename 'id' to 'productId'
        // or ensure your tag helper uses asp-route-id
        public async Task<IActionResult> Details(int productId)
        {
            if (productId <= 0)
            {
                _logger.LogWarning("Details requested with invalid ID: {Id}", productId);
                return NotFound();
            }

            Product? product = await _unitOfWork.Product.GetAsync(
                u => u.Id == productId,
                includeProperties: "Category"
            );

            if (product == null)
            {
                _logger.LogInformation("Product with ID {Id} was not found.", productId);
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Security: Prevents CSRF attacks
        public async Task<IActionResult> Details(Product product)
        {
            // Fix for "ModelState.IsValid should be checked"
            // We pass the model to the action so it can be validated
            if (ModelState.IsValid)
            {
                // Logic for adding to cart goes here
                _logger.LogInformation("Product {Id} added to cart logic triggered.", product.Id);
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload the detail view with current data
            product = await _unitOfWork.Product.GetAsync(u => u.Id == product.Id, includeProperties: "Category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
