using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace aspnetcore_mvc_ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Fiction" },
                    { 2, 2, "Non-Fiction" },
                    { 3, 3, "Science & Technology" },
                    { 4, 4, "Biography" },
                    { 5, 5, "Children's Books" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", 1, "A story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan, set in the Jazz Age on Long Island.", "9780743273565", 29.0, 24.0, 17.0, 20.0, "The Great Gatsby" },
                    { 2, "Yuval Noah Harari", 2, "A brief history of humankind, exploring how Homo sapiens came to dominate the Earth and what the future holds for our species.", "9780062316097", 35.0, 30.0, 22.0, 26.0, "Sapiens" },
                    { 3, "Stephen Hawking", 3, "An exploration of cosmology, black holes, and the nature of time written for general audiences by one of the greatest physicists.", "9780553380163", 40.0, 35.0, 25.0, 30.0, "A Brief History of Time" },
                    { 4, "Walter Isaacson", 4, "The exclusive biography of Steve Jobs, based on more than forty interviews with Jobs conducted over two years.", "9781451648539", 45.0, 40.0, 30.0, 35.0, "Steve Jobs" },
                    { 5, "Antoine de Saint-Exupéry", 5, "A poetic tale about a young prince who travels the universe and learns about life, love, and loss.", "9780156012195", 20.0, 17.0, 12.0, 14.0, "The Little Prince" },
                    { 6, "George Orwell", 5, "A dystopian novel set in a totalitarian society where Big Brother watches your every move and independent thinking is a crime.", "9780451524935", 25.0, 21.0, 15.0, 18.0, "1984" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
