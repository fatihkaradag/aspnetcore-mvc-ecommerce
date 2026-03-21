using aspnetcore_mvc_ecommerce.WebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_mvc_ecommerce.WebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "T-Shirts", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Jeans", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Shoes", DisplayOrder = 3 }
            );
        }
    }
}
