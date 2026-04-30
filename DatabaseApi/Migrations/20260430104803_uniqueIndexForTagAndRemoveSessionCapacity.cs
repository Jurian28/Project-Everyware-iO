using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class uniqueIndexForTagAndRemoveSessionCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_IdEvent",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IdEvent_Title",
                table: "Tags",
                columns: new[] { "IdEvent", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_IdEvent_Title",
                table: "Tags");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 1,
                column: "Capacity",
                value: null);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 2,
                column: "Capacity",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 3,
                column: "Capacity",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IdEvent",
                table: "Tags",
                column: "IdEvent");
        }
    }
}
