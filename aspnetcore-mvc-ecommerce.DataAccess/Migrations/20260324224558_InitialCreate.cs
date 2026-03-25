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
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "ImageUrl", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Eleanor Voss", 1, "A gripping tale of a young woman who discovers a hidden world beneath the streets of a crumbling city, where secrets of the past threaten to unravel everything she has ever known.", "9781234560001", "", 29.0, 24.0, 17.0, 20.0, "The Crimson Horizon" },
                    { 2, "Marcus J. Aldren", 2, "A fascinating journey through the rise of civilizations, exploring how culture, cooperation, and conflict shaped the modern world and what it truly means to be human.", "9781234560002", "", 35.0, 30.0, 22.0, 26.0, "The Human Blueprint" },
                    { 3, "Dr. Lena Hartwell", 3, "An accessible and mind-bending exploration of black holes, quantum mechanics, and the fabric of spacetime, written for curious minds who dare to question the nature of reality.", "9781234560003", "", 40.0, 35.0, 25.0, 30.0, "Beyond the Event Horizon" },
                    { 4, "Sandra K. Mercer", 4, "The untold story of a tech visionary who built an empire from a garage startup, transformed three industries, and redefined what it means to lead with obsession and purpose.", "9781234560004", "", 45.0, 40.0, 30.0, 35.0, "Wired to Win" },
                    { 5, "Olivia Trent", 5, "A magical adventure about a curious little fox named Ziggy who plants seeds among the stars and discovers that friendship and kindness make the whole universe bloom.", "9781234560005", "", 20.0, 17.0, 12.0, 14.0, "Ziggy and the Star Garden" },
                    { 6, "Nathan Cross", 1, "In a world where every thought is monitored and every word is logged, one programmer stumbles upon a forbidden algorithm that could either free humanity or destroy it forever.", "9781234560006", "", 25.0, 21.0, 15.0, 18.0, "The Silent Protocol" }
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
