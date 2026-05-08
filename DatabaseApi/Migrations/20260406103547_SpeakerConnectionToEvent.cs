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
                column: "IdEvent",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 2,
                column: "IdEvent",
                value: 1);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Speakers_IdEvent'
                      AND object_id = OBJECT_ID(N'[Speakers]')
                )
                BEGIN
                    CREATE INDEX [IX_Speakers_IdEvent] ON [Speakers] ([IdEvent]);
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Speakers_Events_IdEvent'
                )
                BEGIN
                    ALTER TABLE [Speakers]
                    ADD CONSTRAINT [FK_Speakers_Events_IdEvent]
                    FOREIGN KEY ([IdEvent]) REFERENCES [Events] ([IdEvent]) ON DELETE NO ACTION;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Speakers_Events_IdEvent'
                )
                BEGIN
                    ALTER TABLE [Speakers] DROP CONSTRAINT [FK_Speakers_Events_IdEvent];
                END
                """);

            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Speakers_IdEvent'
                      AND object_id = OBJECT_ID(N'[Speakers]')
                )
                BEGIN
                    DROP INDEX [IX_Speakers_IdEvent] ON [Speakers];
                END
                """);

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Speakers', 'IdEvent') IS NOT NULL
                BEGIN
                    ALTER TABLE [Speakers] DROP COLUMN [IdEvent];
                END
                """);
        }
    }
}
