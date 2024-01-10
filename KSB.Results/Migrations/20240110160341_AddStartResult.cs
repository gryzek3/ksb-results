using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSB.Results.Migrations
{
    /// <inheritdoc />
    public partial class AddStartResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StartResults_Courses_CourseId",
                table: "StartResults");

            migrationBuilder.DropForeignKey(
                name: "FK_StartResults_Players_PlayerId",
                table: "StartResults");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_StartResults_CourseId",
                table: "StartResults");

            migrationBuilder.DropIndex(
                name: "IX_StartResults_PlayerId",
                table: "StartResults");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "StartResults");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "StartResults");

            migrationBuilder.AddColumn<string>(
                name: "Course",
                table: "StartResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Player",
                table: "StartResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StartResults_TimeStamp",
                table: "StartResults",
                column: "TimeStamp",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StartResults_TimeStamp",
                table: "StartResults");

            migrationBuilder.DropColumn(
                name: "Course",
                table: "StartResults");

            migrationBuilder.DropColumn(
                name: "Player",
                table: "StartResults");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "StartResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "StartResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpreadSheetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surnamename = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StartResults_CourseId",
                table: "StartResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StartResults_PlayerId",
                table: "StartResults",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_StartResults_Courses_CourseId",
                table: "StartResults",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StartResults_Players_PlayerId",
                table: "StartResults",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
