
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
                // Seed data for initial products
                new Product
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Description = "A story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan, set in the Jazz Age on Long Island.",
                    ISBN = "9780743273565",
                    ListPrice = 29,
                    Price = 24,
                    Price50 = 20,
                    Price100 = 17,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "Sapiens",
                    Author = "Yuval Noah Harari",
                    Description = "A brief history of humankind, exploring how Homo sapiens came to dominate the Earth and what the future holds for our species.",
                    ISBN = "9780062316097",
                    ListPrice = 35,
                    Price = 30,
                    Price50 = 26,
                    Price100 = 22,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Title = "A Brief History of Time",
                    Author = "Stephen Hawking",
                    Description = "An exploration of cosmology, black holes, and the nature of time written for general audiences by one of the greatest physicists.",
                    ISBN = "9780553380163",
                    ListPrice = 40,
                    Price = 35,
                    Price50 = 30,
                    Price100 = 25,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Title = "Steve Jobs",
                    Author = "Walter Isaacson",
                    Description = "The exclusive biography of Steve Jobs, based on more than forty interviews with Jobs conducted over two years.",
                    ISBN = "9781451648539",
                    ListPrice = 45,
                    Price = 40,
                    Price50 = 35,
                    Price100 = 30,
                    CategoryId = 4
                },
                new Product
                {
                    Id = 5,
                    Title = "The Little Prince",
                    Author = "Antoine de Saint-Exupéry",
                    Description = "A poetic tale about a young prince who travels the universe and learns about life, love, and loss.",
                    ISBN = "9780156012195",
                    ListPrice = 20,
                    Price = 17,
                    Price50 = 14,
                    Price100 = 12,
                    CategoryId = 5
                },
                new Product
                {
                    Id = 6,
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel set in a totalitarian society where Big Brother watches your every move and independent thinking is a crime.",
                    ISBN = "9780451524935",
                    ListPrice = 25,
                    Price = 21,
                    Price50 = 18,
                    Price100 = 15,
                    CategoryId = 5
                }
            );
        }
    }
}
