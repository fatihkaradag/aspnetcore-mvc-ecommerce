namespace aspnetcore_mvc_ecommerce.Models.ViewModels
{
    // ViewModel for the shopping cart page — combines cart items with order total
    public class ShoppingCartVM
    {
        // List of shopping cart items for the current user
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; } = new List<ShoppingCart>();

        public OrderHeader OrderHeader { get; set; }

    }
}