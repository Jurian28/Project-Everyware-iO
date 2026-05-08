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
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Speakers', 'description') IS NOT NULL
                BEGIN
                    EXEC sp_rename N'[Speakers].[description]', N'Description', 'COLUMN';
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Speakers', 'IdEvent') IS NULL
                BEGIN
                    ALTER TABLE [Speakers] ADD [IdEvent] int NOT NULL DEFAULT 0;
                END
                """);

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
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Speakers', 'IdEvent') IS NOT NULL
                BEGIN
                    ALTER TABLE [Speakers] DROP COLUMN [IdEvent];
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Speakers', 'Description') IS NOT NULL
                BEGIN
                    EXEC sp_rename N'[Speakers].[Description]', N'description', 'COLUMN';
                END
                """);

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
