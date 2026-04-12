using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BgituGrades.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    StoredHash = table.Column<string>(type: "text", nullable: true),
                    LookupHash = table.Column<string>(type: "text", nullable: true),
                    OwnerName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StudyStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StudyEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartWeekNumber = table.Column<int>(type: "integer", nullable: false),
                    CourseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    OfficialStudentId = table.Column<int>(type: "integer", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: false),
                    GroupCourseNumber = table.Column<int>(type: "integer", nullable: false),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    DisciplineName = table.Column<string>(type: "text", nullable: false),
                    Presences = table.Column<string>(type: "text", nullable: false),
                    Marks = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CalendarUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WeekDay = table.Column<int>(type: "integer", nullable: false),
                    Weeknumber = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classes_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfficialId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    OfficialGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IssuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Works_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Works_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfers_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsPresent = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    DisciplineId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presences_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presences_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presences_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IsOverdue = table.Column<bool>(type: "boolean", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    WorkId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marks_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKey_GroupId",
                table: "ApiKeys",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKey_LookupHash",
                table: "ApiKeys",
                column: "LookupHash");

            migrationBuilder.CreateIndex(
                name: "IX_Class_DisciplineId",
                table: "Classes",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_GroupId",
                table: "Classes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_GroupId_DisciplineId",
                table: "Classes",
                columns: new[] { "GroupId", "DisciplineId" });

            migrationBuilder.CreateIndex(
                name: "IX_Mark_StudentId",
                table: "Marks",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Mark_StudentId_WorkId",
                table: "Marks",
                columns: new[] { "StudentId", "WorkId" });

            migrationBuilder.CreateIndex(
                name: "IX_Mark_WorkId",
                table: "Marks",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_ClassId",
                table: "Presences",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_DisciplineId",
                table: "Presences",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_StudentId",
                table: "Presences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_StudentId_Date",
                table: "Presences",
                columns: new[] { "StudentId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportSnapshot_DisciplineId",
                table: "ReportSnapshots",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSnapshot_GroupId",
                table: "ReportSnapshots",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSnapshot_GroupId_DisciplineId",
                table: "ReportSnapshots",
                columns: new[] { "GroupId", "DisciplineId" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_GroupId",
                table: "Students",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_ClassId",
                table: "Transfers",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_DisciplineId",
                table: "Transfers",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_GroupId",
                table: "Transfers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Work_DisciplineId",
                table: "Works",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Work_DisciplineId_GroupId",
                table: "Works",
                columns: new[] { "DisciplineId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_Work_GroupId",
                table: "Works",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "Presences");

            migrationBuilder.DropTable(
                name: "ReportSnapshots");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Works");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
