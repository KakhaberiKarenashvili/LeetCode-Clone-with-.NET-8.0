using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace informaticsge.Migrations
{
    /// <inheritdoc />
    public partial class newproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Problems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MemoryLimit",
                table: "Problems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Problems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "MemoryLimit",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Problems");
        }
    }
}
