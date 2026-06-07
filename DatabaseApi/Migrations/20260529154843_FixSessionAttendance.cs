using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class FixSessionAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionAttendances",
                table: "SessionAttendances");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SessionAttendances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Drop and recreate IdSession column to remove IDENTITY constraint
            migrationBuilder.Sql("ALTER TABLE [SessionAttendances] DROP COLUMN [IdSession]");
            migrationBuilder.AddColumn<int>(
                name: "IdSession",
                table: "SessionAttendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionAttendances",
                table: "SessionAttendances",
                columns: new[] { "IdSession", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionAttendances",
                table: "SessionAttendances");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SessionAttendances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Drop and recreate IdSession column with IDENTITY constraint
            migrationBuilder.Sql("ALTER TABLE [SessionAttendances] DROP COLUMN [IdSession]");
            migrationBuilder.AddColumn<int>(
                name: "IdSession",
                table: "SessionAttendances",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionAttendances",
                table: "SessionAttendances",
                column: "IdSession");
        }
    }
}
