using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository
{
    // UnitOfWork implementation — manages all repositories and database transactions
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        // repositories instance accessible across the application
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            // Initializes repositories with the shared database context
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
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