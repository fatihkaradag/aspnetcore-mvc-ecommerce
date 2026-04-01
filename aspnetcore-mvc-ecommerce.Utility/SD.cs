namespace aspnetcore_mvc_ecommerce.Utility
{
    // Static class for application-wide constants — SD stands for Static Details
    public static class SD
    {
        // User role constants — used for authorization throughout the application
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";

        // Order status constants — used in OrderHeader.OrderStatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        // Payment status constants — used in OrderHeader.PaymentStatus
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        // Session key constants — prevents magic strings across the application
        public const string SessionCart = "SessionShoppingCart";
    }
}