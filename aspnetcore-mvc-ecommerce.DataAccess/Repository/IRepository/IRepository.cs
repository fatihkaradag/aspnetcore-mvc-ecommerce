using System.Linq.Expressions;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // Generic repository interface — defines common CRUD operations for all entities
    public interface IRepository<T> where T : class
    {
        // Retrieves all records synchronously, optionally including related entities
        IEnumerable<T> GetAll(string? includeProperties = null);

        // Retrieves all records asynchronously, optionally including related entities
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);

        // Retrieves a single record matching the filter synchronously
        T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null);

        // Retrieves a single record matching the filter asynchronously
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        // Adds a new entity to the database context
        void Add(T entity);

        // Removes a single entity from the database context
        void Remove(T entity);

        // Removes a collection of entities from the database context
        void RemoveRange(IEnumerable<T> entity);
    }
}