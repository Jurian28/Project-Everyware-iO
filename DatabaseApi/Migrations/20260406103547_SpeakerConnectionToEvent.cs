using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class SpeakerConnectionToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEvent",
                table: "Speakers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 1,
                column: "IdEvent",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 2,
                column: "IdEvent",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_IdEvent",
                table: "Speakers",
                column: "IdEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_Speakers_Events_IdEvent",
                table: "Speakers",
                column: "IdEvent",
                principalTable: "Events",
                principalColumn: "IdEvent",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Speakers_Events_IdEvent",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_IdEvent",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "IdEvent",
                table: "Speakers");
        }
    }
}
