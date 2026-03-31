using aspnetcore_mvc_ecommerce.Models;

namespace aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository
{
    // ApplicationUser-specific repository interface — extends generic IRepository
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
    }
}