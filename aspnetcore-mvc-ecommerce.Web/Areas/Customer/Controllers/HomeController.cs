using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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

        public async Task<IActionResult> Index()
        {
            // Resolve authenticated user id from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                int cartCount = (await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.ApplicationUserId == claim.Value
                )).Count();

                HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

            }


            IEnumerable<Product> productList = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
            return View(productList);
        }

        // GET: /Home/Details — retrieves product by id and returns detail page
        public async Task<IActionResult> Details(int productId)
        {
            if (productId <= 0)
            {
                _logger.LogWarning("Details requested with invalid product ID: {Id}", productId);
                return NotFound();
            }

            // Fetches product with category info asynchronously via repository
            Product? product = await _unitOfWork.Product.GetAsync(
                u => u.Id == productId,
                includeProperties: "Category"
            );

            if (product == null)
            {
                _logger.LogInformation("Product with ID {Id} was not found.", productId);
                return NotFound();
            }

            // Wraps product in a ShoppingCart for the detail view
            ShoppingCart cartItem = new()
            {
                Product = product,
                ProductId = product.Id,
                Quantity = 1
            };

            return View(cartItem);
        }

        // POST: /Home/Details — adds product to shopping cart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            // Resolve authenticated user id from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Reject if user identity cannot be resolved
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Details POST: Could not resolve userId from claims.");
                return Unauthorized();
            }

            shoppingCart.ApplicationUserId = userId;

            if (!ModelState.IsValid)
            {
                // Reload product with category for view re-render on validation failure
                shoppingCart.Product = await _unitOfWork.Product.GetAsync(
                    u => u.Id == shoppingCart.ProductId,
                    includeProperties: "Category"
                );
                return View(shoppingCart);
            }

            // Check if this product already exists in the user's cart
            ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId
            );

            if (cartFromDb != null)
            {
                // Increment quantity if product already in cart
                cartFromDb.Quantity += shoppingCart.Quantity;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                // Add new cart line item
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            await _unitOfWork.SaveAsync();

            // Update session cart count after every cart modification
            int cartCount = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId
            )).Count();

            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

            TempData["success"] = "Cart updated successfully.";
            return RedirectToAction(nameof(Index));
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
