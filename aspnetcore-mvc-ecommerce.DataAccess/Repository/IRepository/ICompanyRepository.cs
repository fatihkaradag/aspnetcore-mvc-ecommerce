using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // Company-specific repository interface — extends generic IRepository with update operation
    public interface ICompanyRepository : IRepository<Company>
    {
        // Updates an existing company record in the database
        void Update(Company obj);
    }
}