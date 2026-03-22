using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // Category-specific repository interface — extends generic IRepository with update operation
    public interface ICategoryRepository : IRepository<Category>
    {
        // Updates an existing category record in the database
        void Update(Category obj);

    }
}