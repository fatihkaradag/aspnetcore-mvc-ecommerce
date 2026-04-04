using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using aspnetcore_mvc_ecommerce.Models.ViewModels;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Customer.Controllers
{
    // Handles all HTTP requests related to shopping cart management
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;

        // Bound property for the shopping cart ViewModel — shared across actions
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; } = new();

        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /Cart/Index — retrieves and displays the current user's shopping cart
        public async Task<IActionResult> Index()
        {
            // Retrieves the logged-in user's unique identifier from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // Initializes the ViewModel with user-specific cart items including product info
            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new()
            };

            // Calculates price and order total based on quantity tier pricing
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Quantity;
            }

            return View(ShoppingCartVM);
        }

        // GET: /Cart/Summary — displays order summary before checkout with pre-filled user details
        public async Task<IActionResult> Summary()
        {
            // Retrieves the logged-in user's unique identifier from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // Initializes the ViewModel with user-specific cart items including product info
            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new()
            };

            // Fetches the application user to pre-fill shipping details
            ApplicationUser? applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            if (applicationUser != null)
            {
                // Pre-fills order header with user's saved address details
                ShoppingCartVM.OrderHeader.Name = applicationUser.Name;
                ShoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber ?? string.Empty;
                ShoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddress ?? string.Empty;
                ShoppingCartVM.OrderHeader.City = applicationUser.City ?? string.Empty;
                ShoppingCartVM.OrderHeader.State = applicationUser.State ?? string.Empty;
                ShoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode ?? string.Empty;
            }

            // Calculates price and order total based on quantity tier pricing
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Quantity;
            }

            return View(ShoppingCartVM);
        }

        // POST: /Cart/Summary — processes order placement and redirects to confirmation
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST()
        {
            // Retrieves the logged-in user's unique identifier from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // Fetches the application user to check company membership
            ApplicationUser? applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);
            if (applicationUser == null) return NotFound();

            // Fetches user-specific cart items including product info
            ShoppingCartVM.ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product"
            );

            // Sets order header metadata
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            // Calculates price and order total based on quantity tier pricing
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Quantity;
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // Individual customer — payment required immediately
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // Company customer — delayed payment approved, no immediate payment required
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            // Saves order header to database before creating Stripe session
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            // Creates order detail records for each cart item
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Quantity = cart.Quantity
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }

            // Saves all order details to database
            await _unitOfWork.SaveAsync();

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // Individual customer — creates Stripe checkout session for immediate payment
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    // Redirects to order confirmation on success
                    SuccessUrl = domain + $"/customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    // Redirects back to cart on cancel
                    CancelUrl = domain + "/customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                // Builds Stripe line items from cart
                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            // Stripe requires amount in smallest currency unit — cents for USD
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product!.Title
                            }
                        },
                        Quantity = item.Quantity
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                // Creates Stripe checkout session and saves session ID to order
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(
                    ShoppingCartVM.OrderHeader.Id,
                    session.Id,
                    session.PaymentIntentId
                );
                await _unitOfWork.SaveAsync();

                // Redirects to Stripe checkout page using 303 See Other
                Response.Headers.Location = session.Url;
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        // GET: /Cart/OrderConfirmation — verifies payment and clears cart after order
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            // Fetches order header with user info to verify payment status
            OrderHeader? orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                u => u.Id == id,
                includeProperties: "ApplicationUser"
            );

            if (orderHeader == null) return NotFound();

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // Individual customer — verifies Stripe payment was completed
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // Updates order with confirmed payment intent ID and approves order
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.SaveAsync();
                }
            }

            // Clears the user's shopping cart after successful order placement
            List<ShoppingCart> shoppingCarts = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == orderHeader.ApplicationUserId
            )).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            await _unitOfWork.SaveAsync();

            return View(id);
        }

        // GET: /Cart/Plus — increases cart item quantity by 1
        public async Task<IActionResult> Plus(int cartId)
        {
            // Fetches cart item asynchronously via repository
            ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);
            if (cartFromDb == null) return NotFound();

            // Increments quantity and persists changes
            cartFromDb.Quantity += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Cart/Minus — decreases cart item quantity by 1 or removes if quantity is 1
        public async Task<IActionResult> Minus(int cartId)
        {
            // Fetches cart item asynchronously via repository
            ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);
            if (cartFromDb == null) return NotFound();

            if (cartFromDb.Quantity <= 1)
            {
                // Removes item from cart if quantity would drop below 1
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                // Decrements quantity and persists changes
                cartFromDb.Quantity -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cart/Remove — removes item from cart entirely
        public async Task<IActionResult> Remove(int cartId)
        {
            // Fetches cart item asynchronously via repository before removing
            ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);
            if (cartFromDb == null) return NotFound();

            // Removes cart item and persists changes
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // Calculates product price based on quantity tier pricing logic
        private static double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Quantity <= 50)
            {
                // Standard price for 1-50 units
                return shoppingCart.Product!.Price;
            }
            else if (shoppingCart.Quantity <= 100)
            {
                // Discounted price for 51-100 units
                return shoppingCart.Product!.Price50;
            }
            else
            {
                // Best price for 100+ units
                return shoppingCart.Product!.Price100;
            }
        }
    }
}