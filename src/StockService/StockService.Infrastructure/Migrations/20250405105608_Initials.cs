using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedAt", "Name", "ProductId", "Quantity", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"), new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Apple Watch 44mm", new Guid("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"), 100, 150.75m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"), new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Nvidia RTX 5080", new Guid("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"), 50, 250.50m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"), new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Apple Iphone 14 Pro", new Guid("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"), 150, 85000m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
