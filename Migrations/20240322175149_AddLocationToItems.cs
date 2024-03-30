using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Supershop.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                table: "items");
        }
    }
}
