using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueRoomLabelsPerEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_IdEvent",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "RoomLabel",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdEvent_RoomLabel",
                table: "Rooms",
                columns: new[] { "IdEvent", "RoomLabel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_IdEvent_RoomLabel",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "RoomLabel",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdEvent",
                table: "Rooms",
                column: "IdEvent");
        }
    }
}
