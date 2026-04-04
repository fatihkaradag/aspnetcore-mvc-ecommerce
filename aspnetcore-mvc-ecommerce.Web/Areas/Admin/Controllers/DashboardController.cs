using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    // Handles the admin dashboard — displays store summary statistics
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")] // Restricts access to admin users only
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /Admin/Dashboard/Index — loads and displays store summary metrics
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetches counts asynchronously for dashboard stats
                ViewBag.ProductCount = (await _unitOfWork.Product.GetAllAsync()).Count();
                ViewBag.CategoryCount = (await _unitOfWork.Category.GetAllAsync()).Count();
                ViewBag.CompanyCount = (await _unitOfWork.Company.GetAllAsync()).Count();

                ViewBag.OrderCount = (await _unitOfWork.OrderHeader.GetAllAsync()).Count();

                _logger.LogInformation("Dashboard metrics loaded successfully at {Time}", DateTime.UtcNow);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load admin dashboard metrics.");
                return View("Error");
            }
        }
    }
}