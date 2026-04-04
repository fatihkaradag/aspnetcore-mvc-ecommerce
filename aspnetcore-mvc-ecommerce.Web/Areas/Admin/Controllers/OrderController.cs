using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using aspnetcore_mvc_ecommerce.Models.ViewModels;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;

        // Bound from form post; used across POST actions
        [BindProperty]
        public OrderVM OrderVM { get; set; } = new();

        public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /Admin/Order/Index — renders the order management view
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/Order/Details/{orderId} — loads order header and line items
        public async Task<IActionResult> Details(int orderId)
        {
            // Reject invalid id early
            if (orderId <= 0) return BadRequest();

            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                u => u.Id == orderId,
                includeProperties: "ApplicationUser"
            );

            // Return 404 if the order does not exist
            if (orderHeader == null)
            {
                _logger.LogWarning("Order {OrderId} not found.", orderId);
                return NotFound();
            }

            // Build view model with header and detail lines
            OrderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetails = await _unitOfWork.OrderDetail.GetAllAsync(
                    u => u.OrderHeaderId == orderId,
                    includeProperties: "Product"
                )
            };

            return View(OrderVM);
        }

        // POST: /Admin/Order/UpdateOrderDetail — updates shipping and carrier info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetail()
        {
            // Fetch tracked entity from database to prevent overposting
            var orderHeaderFromDb = await _unitOfWork.OrderHeader.GetAsync(
                u => u.Id == OrderVM.OrderHeader.Id
            );

            // Guard against manipulated hidden-field id
            if (orderHeaderFromDb == null)
            {
                _logger.LogWarning("UpdateOrderDetail: Order {OrderId} not found.", OrderVM.OrderHeader.Id);
                return NotFound();
            }

            // Apply only the fields the form is permitted to change
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            // Only update carrier when explicitly provided
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;

            // Only update tracking number when explicitly provided
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Order {OrderId} details updated.", orderHeaderFromDb.Id);
            TempData["success"] = "Order details updated successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        // POST: /Admin/Order/StartProcessing — transitions order status to InProcess
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcessing()
        {
            // Verify order exists before updating status
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null)
            {
                _logger.LogWarning("StartProcessing: Order {OrderId} not found.", OrderVM.OrderHeader.Id);
                return NotFound();
            }

            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Order {OrderId} moved to InProcess.", OrderVM.OrderHeader.Id);
            TempData["success"] = "Order status updated to Processing.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        // POST: /Admin/Order/ShipOrder — marks order as shipped and sets shipping date
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == OrderVM.OrderHeader.Id);

            // Guard against non-existent order
            if (orderHeader == null)
            {
                _logger.LogWarning("ShipOrder: Order {OrderId} not found.", OrderVM.OrderHeader.Id);
                return NotFound();
            }

            // Apply tracking and carrier from form
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.UtcNow;

            // Set 30-day payment due date for company/delayed-payment orders
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
                orderHeader.PaymentDueDate = DateTime.UtcNow.AddDays(30);

            _unitOfWork.OrderHeader.Update(orderHeader);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Order {OrderId} shipped.", orderHeader.Id);
            TempData["success"] = "Order shipped successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        // POST: /Admin/Order/CancelOrder — cancels order and refunds via Stripe if already paid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == OrderVM.OrderHeader.Id);

            // Guard against non-existent order
            if (orderHeader == null)
            {
                _logger.LogWarning("CancelOrder: Order {OrderId} not found.", OrderVM.OrderHeader.Id);
                return NotFound();
            }

            // Issue Stripe refund if payment was already captured
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new Stripe.RefundCreateOptions
                {
                    Reason = Stripe.RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new Stripe.RefundService();
                await service.CreateAsync(options);

                // Mark order as cancelled with refunded payment status
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
                _logger.LogInformation("Order {OrderId} cancelled and refunded.", orderHeader.Id);
            }
            else
            {
                // No payment captured — cancel without refund
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                _logger.LogInformation("Order {OrderId} cancelled (no refund).", orderHeader.Id);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Order cancelled successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        // POST: /Admin/Order/Details — initiates Stripe checkout for delayed-payment orders
        // POST: /Admin/Order/Details — initiates Stripe checkout for delayed-payment orders
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details_PAY_NOW()
        {
            // Reload full OrderVM from DB to ensure data integrity
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                u => u.Id == OrderVM.OrderHeader.Id,
                includeProperties: "ApplicationUser"
            );

            if (orderHeader == null)
            {
                _logger.LogWarning("Details_PAY_NOW: Order {OrderId} not found.", OrderVM.OrderHeader.Id);
                return NotFound();
            }

            OrderVM.OrderHeader = orderHeader;
            OrderVM.OrderDetails = await _unitOfWork.OrderDetail.GetAllAsync(
                u => u.OrderHeaderId == OrderVM.OrderHeader.Id,
                includeProperties: "Product"
            );

            // Build base domain URL for Stripe success/cancel redirects
            var domain = $"{Request.Scheme}://{Request.Host.Value}/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            // Build Stripe line items from order details
            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Convert dollars to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product?.Title ?? "Unknown Product"
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            // Create Stripe session and persist session/intent IDs
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // ✅ Tutarlı method adı — UpdateStripePaymentId (Id, büyük harf D değil)
            _unitOfWork.OrderHeader.UpdateStripePaymentId(
                OrderVM.OrderHeader.Id,
                session.Id,
                session.PaymentIntentId
            );
            await _unitOfWork.SaveAsync();

            // Redirect to Stripe checkout page (303 See Other)
            Response.Headers.Location = session.Url;
            return new StatusCodeResult(303);
        }

        // GET: /Admin/Order/PaymentConfirmation — verifies Stripe payment for company orders
        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {
            // Reject invalid id early
            if (orderHeaderId <= 0) return BadRequest();

            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                u => u.Id == orderHeaderId
            );

            if (orderHeader == null)
            {
                _logger.LogWarning("PaymentConfirmation: Order {OrderId} not found.", orderHeaderId);
                return NotFound();
            }

            // Only company/delayed-payment orders need Stripe verification here
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new Stripe.Checkout.SessionService();

                // Fetch Stripe session to verify payment completion
                Stripe.Checkout.Session session = await service.GetAsync(orderHeader.SessionId);

                if (session.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
                {
                    // Persist confirmed payment intent and approve payment status
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(
                        orderHeaderId,
                        session.Id,
                        session.PaymentIntentId
                    );
                    _unitOfWork.OrderHeader.UpdateStatus(
                        orderHeaderId,
                        orderHeader.OrderStatus,
                        SD.PaymentStatusApproved
                    );
                    await _unitOfWork.SaveAsync();

                    _logger.LogInformation("Order {OrderId} payment confirmed via Stripe.", orderHeaderId);
                }
            }

            return View(orderHeaderId);
        }

        #region API CALLS

        // GET: /Admin/Order/GetAll?status={status} — returns filtered order list as JSON for DataTables
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;

            // Admin and Employee see all orders; others see only their own
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = await _unitOfWork.OrderHeader.GetAllAsync(
                    includeProperties: "ApplicationUser"
                );
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity!;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Reject unauthenticated or malformed identity
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetAll: Could not resolve userId from claims.");
                    return Unauthorized();
                }

                objOrderHeaders = await _unitOfWork.OrderHeader.GetAllAsync(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "ApplicationUser"
                );
            }

            // Apply status filter using in-memory LINQ after DB fetch
            objOrderHeaders = status switch
            {
                SD.StatusFilterPending => objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment),
                SD.StatusFilterInProcess => objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess),
                SD.StatusFilterCompleted => objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped),
                SD.StatusFilterApproved => objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved),
                _ => objOrderHeaders
            };

            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}