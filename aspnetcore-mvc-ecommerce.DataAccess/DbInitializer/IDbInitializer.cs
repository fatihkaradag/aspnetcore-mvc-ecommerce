namespace aspnetcore_mvc_ecommerce.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        // Applies pending migrations and seeds default roles and users
        Task InitializeAsync();
    }
}