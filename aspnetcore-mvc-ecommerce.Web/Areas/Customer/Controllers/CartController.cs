using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models.ViewModels;
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve the logged-in user's unique identifier
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // Initialize the ViewModel with user-specific cart items
            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderTotal = 0,
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Calculate price based on quantity (Tier Pricing Logic)
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Quantity);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            
            return View();
        }


        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);
            cartFromDb.Result.Quantity += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb.Result);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);

            if(cartFromDb.Result.Quantity <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb.Result);
            }
            else
            {
                cartFromDb.Result.Quantity -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb.Result);
            }

            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(cartFromDb.Result);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Quantity <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Quantity <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }


    }
}