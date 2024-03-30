using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Supershop.Migrations
{
    /// <inheritdoc />
    public partial class senditemstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "Id", "Count", "Name", "Type", "location" },
                values: new object[,]
                {
                    { 1, 0, "Pepsi", "Softdrinks", "A1" },
                    { 2, 0, "Fanta", "Softdrinks", "A1" },
                    { 3, 0, "7up", "Softdrinks", "A1" },
                    { 4, 0, "Cockacola", "Softdrinks", "A1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
