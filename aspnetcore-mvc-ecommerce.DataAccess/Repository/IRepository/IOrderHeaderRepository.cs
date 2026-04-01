using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // OrderHeader-specific repository interface — extends generic IRepository with order management operations
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        // Updates an existing order header record in the database
        void Update(OrderHeader obj);

        // Updates only the order and payment status fields
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        // Updates Stripe session and payment intent IDs after payment processing
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}