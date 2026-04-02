using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BgituGrades.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePresence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Presences_ClassId",
                table: "Presences",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_Classes_ClassId",
                table: "Presences",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_Classes_ClassId",
                table: "Presences");

            migrationBuilder.DropIndex(
                name: "IX_Presences_ClassId",
                table: "Presences");
        }
    }
}
