using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository
{
    // Generic repository implementation — provides common CRUD operations for all entities
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        // DbSet used to query and save instances of type T
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        // Adds a new entity to the database context
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        // Retrieves a single entity synchronously matching the given filter expression
        public T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            // Dynamically include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return query.FirstOrDefault();
        }

        // Retrieves a single entity asynchronously matching the given filter expression
        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);

            // Dynamically include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        // Retrieves all entities synchronously, optionally including related navigation properties
        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // Dynamically include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return query.ToList();
        }

        // Retrieves all entities asynchronously, optionally including related navigation properties
        public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // Dynamically include related entities if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.ToListAsync();
        }

        // Removes a single entity from the database context
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        // Removes a collection of entities from the database context
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}