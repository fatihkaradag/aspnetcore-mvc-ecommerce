using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Administrative Dashboard for e-commerce management.
    /// Provides summaries of products, categories, and potential orders.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users only
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Displays the summary of the store status.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // In an enterprise app, we calculate summary statistics for the dashboard
                // Fetch counts asynchronously to ensure the UI remains responsive
                var productCount = (await _unitOfWork.Product.GetAllAsync()).Count();
                var categoryCount = (await _unitOfWork.Category.GetAllAsync()).Count();

                // Using ViewBag for simple dashboard stats (In larger apps, use a DashboardVM)
                ViewBag.ProductCount = productCount;
                ViewBag.CategoryCount = categoryCount;

                _logger.LogInformation("Dashboard metrics loaded successfully at {Time}", DateTime.Now);

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