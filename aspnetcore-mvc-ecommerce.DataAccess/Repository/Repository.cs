using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

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
        public T? Get(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false)
        {

            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

            // Ensure filter is applied only if provided to prevent potential null reference exceptions
            if (filter != null)
            {
                query = query.Where(filter);
            }


            // Process include properties with trimming to avoid "property not found" errors due to whitespaces
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim()); // Added .Trim() for robustness
                }
            }

            return query.FirstOrDefault();
        }

        // Retrieves a single entity asynchronously matching the given filter expression
        public async Task<T?> GetAsync(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false)
        {

            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            // Task-based asynchronous execution to keep the UI/Thread responsive
            return await query.FirstOrDefaultAsync();
        }

        // Retrieves all entities synchronously, optionally including related navigation properties
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter,string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

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
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter,string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

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