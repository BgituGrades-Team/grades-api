using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BgituGrades.Migrations
{
    /// <inheritdoc />
    public partial class AddSubGroupToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubGroup",
                table: "Groups",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubGroup",
                table: "Groups");
        }
    }
}
