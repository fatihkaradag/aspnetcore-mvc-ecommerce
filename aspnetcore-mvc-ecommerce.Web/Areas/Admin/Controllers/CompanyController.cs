using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_mvc_ecommerce.Web.Areas.Admin.Controllers
{
    // Handles all HTTP requests related to Company management
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restricts access to admin users only
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(IUnitOfWork unitOfWork, ILogger<CompanyController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: /Company/Index — retrieves and displays all companies
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetches all companies asynchronously via repository
                IEnumerable<Company> objCompanyList = await _unitOfWork.Company.GetAllAsync();
                return View(objCompanyList.ToList());
            }
            catch (Exception ex)
            {
                // Logs error and returns 500 if company list cannot be fetched
                _logger.LogError(ex, "Error occurred while fetching companies for Index.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET: /Company/Upsert — returns create form if id is null, edit form if id exists
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create mode — return empty company form
                return View(new Company());
            }
            else
            {
                // Edit mode — fetch existing company and populate form
                Company? companyObj = await _unitOfWork.Company.GetAsync(u => u.Id == id);
                if (companyObj == null) return NotFound();

                return View(companyObj);
            }
        }

        // POST: /Company/Upsert — creates or updates company based on Id value
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                if (companyObj.Id == 0)
                {
                    // Create mode — adds new company to database
                    _unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "Company created successfully.";
                }
                else
                {
                    // Edit mode — updates existing company in database
                    _unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "Company updated successfully.";
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }

            return View(companyObj);
        }

        // GET: /Company/Delete — retrieves company by id and returns delete confirmation form
        public async Task<IActionResult> Delete(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches company asynchronously via repository
            Company? companyFromDb = await _unitOfWork.Company.GetAsync(u => u.Id == id);
            if (companyFromDb == null) return NotFound();

            return View(companyFromDb);
        }

        // POST: /Company/Delete — removes the company from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0) return NotFound();

            // Fetches company asynchronously via repository before removing
            Company? obj = await _unitOfWork.Company.GetAsync(u => u.Id == id);
            if (obj == null) return NotFound();

            // Removes company via repository and persists changes
            _unitOfWork.Company.Remove(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Company deleted successfully.";

            return RedirectToAction("Index");
        }

        #region API CALLS

        // GET: /Company/GetAll — returns all companies as JSON for DataTable API calls
        public async Task<IActionResult> GetAll()
        {
            // Retrieves all companies and returns as JSON
            IEnumerable<Company> objCompanyList = await _unitOfWork.Company.GetAllAsync();
            return Json(new { data = objCompanyList });
        }

        // DELETE: /Company/DeleteAjax — removes company via AJAX call
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int? id)
        {
            // Validates that id is not null or zero
            if (id == null || id == 0)
                return Json(new { success = false, message = "Invalid id" });

            // Fetches company before deletion
            Company? companyToBeDeleted = await _unitOfWork.Company.GetAsync(u => u.Id == id);
            if (companyToBeDeleted == null)
                return Json(new { success = false, message = "Error while deleting" });

            // Removes company and persists changes
            _unitOfWork.Company.Remove(companyToBeDeleted);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Company deleted successfully" });
        }

        #endregion
    }
}