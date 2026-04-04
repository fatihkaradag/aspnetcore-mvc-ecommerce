namespace aspnetcore_mvc_ecommerce.Utility
{
    // Stripe payment gateway configuration — bound from appsettings.json Stripe section
    public class StripeSettings
    {
        // Stripe secret key — used server-side for API calls, never expose to client
        public string SecretKey { get; set; } = string.Empty;

        // Stripe publishable key — used client-side for Stripe.js initialization
        public string PublishableKey { get; set; } = string.Empty;
    }
}