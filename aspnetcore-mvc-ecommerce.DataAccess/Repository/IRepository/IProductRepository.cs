using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // Product-specific repository interface — extends generic IRepository with update operation
    public interface IProductRepository : IRepository<Product>
    {
        // Updates an existing product record in the database
        void Update(Product obj);

    }
}