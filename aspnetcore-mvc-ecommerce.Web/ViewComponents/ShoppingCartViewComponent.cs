using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace aspnetcore_mvc_ecommerce.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Invoked by the view component tag in layout — sets and returns cart count
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                // Fetch live cart count from DB and update session
                int cartCount = (await _unitOfWork.ShoppingCart.GetAllAsync(
                    u => u.ApplicationUserId == claim.Value
                )).Count();

                HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

                return View(cartCount);
            }

            // User is not authenticated — clear session and show zero
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}