using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
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
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            shoppingCart.ApplicationUserId = userId;


            if (ModelState.IsValid)
            {
                ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(
                    u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId
                );

                if (cartFromDb != null)
                {
                    cartFromDb.Quantity += shoppingCart.Quantity;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                else
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }

                await _unitOfWork.SaveAsync();

                TempData["success"] = "Cart updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            shoppingCart.Product = await _unitOfWork.Product.GetAsync(
                u => u.Id == shoppingCart.ProductId,
                includeProperties: "Category"
            );

            return View(shoppingCart);
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
