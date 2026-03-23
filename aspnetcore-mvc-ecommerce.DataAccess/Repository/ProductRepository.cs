using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository
{
    // Product repository — implements product-specific data access operations
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // Updates an existing product entity in the database context
        public void Update(Product obj)
        {
            _db.Products.Update(obj);
        }

    }
}