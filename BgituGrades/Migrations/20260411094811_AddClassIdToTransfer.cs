using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BgituGrades.Migrations
{
    /// <inheritdoc />
    public partial class AddClassIdToTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Transfers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ClassId",
                table: "Transfers",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Classes_ClassId",
                table: "Transfers",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Classes_ClassId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_ClassId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Transfers");
        }
    }
}
