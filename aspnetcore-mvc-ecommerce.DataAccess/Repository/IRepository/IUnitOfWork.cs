namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // UnitOfWork interface — provides access to all repositories through a single entry point
    public interface IUnitOfWork
    {
        // Exposes the Category repository for category-related operations
        ICategoryRepository Category { get; }

        // Persists all pending changes to the database synchronously
        void Save();

        // Persists all pending changes to the database asynchronously
        Task SaveAsync();
    }
}