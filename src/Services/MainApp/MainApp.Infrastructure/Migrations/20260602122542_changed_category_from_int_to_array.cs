using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changed_category_from_int_to_array : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Problems");

            migrationBuilder.AddColumn<int[]>(
                name: "Categories",
                table: "Problems",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Problems");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Problems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
