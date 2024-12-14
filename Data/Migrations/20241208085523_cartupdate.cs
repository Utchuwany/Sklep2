using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep2.Data.Migrations
{
    /// <inheritdoc />
    public partial class cartupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Cart",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Cart");
        }
    }
}
