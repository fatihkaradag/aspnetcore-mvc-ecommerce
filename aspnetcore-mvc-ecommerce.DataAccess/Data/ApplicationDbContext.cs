
using aspnetcore_mvc_ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore_mvc_ecommerce.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                // Seed data for initial book categories
                new Category { Id = 1, Name = "Fiction", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Non-Fiction", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Science & Technology", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Biography", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Children's Books", DisplayOrder = 5 }
            );

            modelBuilder.Entity<Product>().HasData(
                // Seed data for initial products (fully fictional)
                new Product
                {
                    Id = 1,
                    Title = "The Crimson Horizon",
                    Author = "Eleanor Voss",
                    Description = "A gripping tale of a young woman who discovers a hidden world beneath the streets of a crumbling city, where secrets of the past threaten to unravel everything she has ever known.",
                    ISBN = "9781234560001",
                    ListPrice = 29,
                    Price = 24,
                    Price50 = 20,
                    Price100 = 17,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "The Human Blueprint",
                    Author = "Marcus J. Aldren",
                    Description = "A fascinating journey through the rise of civilizations, exploring how culture, cooperation, and conflict shaped the modern world and what it truly means to be human.",
                    ISBN = "9781234560002",
                    ListPrice = 35,
                    Price = 30,
                    Price50 = 26,
                    Price100 = 22,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Title = "Beyond the Event Horizon",
                    Author = "Dr. Lena Hartwell",
                    Description = "An accessible and mind-bending exploration of black holes, quantum mechanics, and the fabric of spacetime, written for curious minds who dare to question the nature of reality.",
                    ISBN = "9781234560003",
                    ListPrice = 40,
                    Price = 35,
                    Price50 = 30,
                    Price100 = 25,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Title = "Wired to Win",
                    Author = "Sandra K. Mercer",
                    Description = "The untold story of a tech visionary who built an empire from a garage startup, transformed three industries, and redefined what it means to lead with obsession and purpose.",
                    ISBN = "9781234560004",
                    ListPrice = 45,
                    Price = 40,
                    Price50 = 35,
                    Price100 = 30,
                    CategoryId = 4
                },
                new Product
                {
                    Id = 5,
                    Title = "Ziggy and the Star Garden",
                    Author = "Olivia Trent",
                    Description = "A magical adventure about a curious little fox named Ziggy who plants seeds among the stars and discovers that friendship and kindness make the whole universe bloom.",
                    ISBN = "9781234560005",
                    ListPrice = 20,
                    Price = 17,
                    Price50 = 14,
                    Price100 = 12,
                    CategoryId = 5
                },
                new Product
                {
                    Id = 6,
                    Title = "The Silent Protocol",
                    Author = "Nathan Cross",
                    Description = "In a world where every thought is monitored and every word is logged, one programmer stumbles upon a forbidden algorithm that could either free humanity or destroy it forever.",
                    ISBN = "9781234560006",
                    ListPrice = 25,
                    Price = 21,
                    Price50 = 18,
                    Price100 = 15,
                    CategoryId = 1
                }
            );
        }
    }
}
