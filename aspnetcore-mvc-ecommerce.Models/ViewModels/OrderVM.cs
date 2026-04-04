namespace aspnetcore_mvc_ecommerce.Models.ViewModels
{
    // ViewModel for order management pages — combines order header with its line items
    public class OrderVM
    {
        // Order header containing customer, shipping and payment information
        public OrderHeader OrderHeader { get; set; } = new();

        // Collection of order line items — each item represents a product in the order
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}