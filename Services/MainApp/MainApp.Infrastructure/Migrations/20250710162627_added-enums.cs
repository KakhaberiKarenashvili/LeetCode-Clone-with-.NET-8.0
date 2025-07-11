using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedenums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Problems");

            migrationBuilder.AlterColumn<int>(
                name: "Difficulty",
                table: "Problems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Problems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Problems");

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "Problems",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Problems",
                type: "text",
                nullable: true);
        }
    }
}
