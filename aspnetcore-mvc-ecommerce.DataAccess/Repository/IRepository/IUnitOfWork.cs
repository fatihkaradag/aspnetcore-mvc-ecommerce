namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // UnitOfWork interface — provides access to all repositories through a single entry point
    public interface IUnitOfWork
    {
        // Exposes repositories for operations
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }

        // Persists all pending changes to the database synchronously
        void Save();

        // Persists all pending changes to the database asynchronously
        Task SaveAsync();
    }
}