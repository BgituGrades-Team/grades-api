using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BgituGrades.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ApiKeys",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ApiKeys");
        }
    }
}
