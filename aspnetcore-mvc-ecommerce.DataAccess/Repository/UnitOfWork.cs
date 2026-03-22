using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository
{
    // UnitOfWork implementation — manages all repositories and database transactions
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        // Category repository instance accessible across the application
        public ICategoryRepository Category { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            // Initializes category repository with the shared database context
            Category = new CategoryRepository(_db);
        }

        // Persists all pending changes to the database synchronously
        public void Save()
        {
            _db.SaveChanges();
        }

        // Persists all pending changes to the database asynchronously
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}