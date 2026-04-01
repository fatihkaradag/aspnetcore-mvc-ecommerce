using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // OrderDetail-specific repository interface — extends generic IRepository with update operation
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        // Updates an existing order detail record in the database
        void Update(OrderDetail obj);
    }
}