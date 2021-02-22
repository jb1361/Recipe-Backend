using Microsoft.EntityFrameworkCore.Migrations;

namespace Recipe.Api.Migrations
{
    public partial class AddAuthorToRecipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Recipe",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Recipe");
        }
    }
}
