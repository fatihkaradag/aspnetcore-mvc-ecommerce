using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // ShoppingCart-specific repository interface — extends generic IRepository with update operation
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        // Updates an existing shopping cart item in the database
        void Update(ShoppingCart obj);
    }
}