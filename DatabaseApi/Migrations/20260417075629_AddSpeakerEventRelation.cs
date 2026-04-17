using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeakerEventRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "Speakers",
                newName: "Description");

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
                columns: new[] { "Description", "IdEvent" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 2,
                columns: new[] { "Description", "IdEvent" },
                values: new object[] { null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdEvent",
                table: "Speakers");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Speakers",
                newName: "description");

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 1,
                column: "description",
                value: "Expert in C# en Cloud.");

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 2,
                column: "description",
                value: "Expert in Databases en networking.");
        }
    }
}
