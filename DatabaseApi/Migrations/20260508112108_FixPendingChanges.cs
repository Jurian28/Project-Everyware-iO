using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Session_has_Speaker_Speaker_SpeakersIdSpeaker')
                BEGIN
                    ALTER TABLE [Session_has_Speaker] DROP CONSTRAINT [FK_Session_has_Speaker_Speaker_SpeakersIdSpeaker];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Speaker_Events_IdEvent')
                BEGIN
                    ALTER TABLE [Speaker] DROP CONSTRAINT [FK_Speaker_Events_IdEvent];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1 FROM sys.key_constraints
                       WHERE [name] = 'PK_Speaker' AND [type] = 'PK'
                   )
                BEGIN
                    ALTER TABLE [Speaker] DROP CONSTRAINT [PK_Speaker];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                BEGIN
                    DELETE FROM [Speaker] WHERE [IdSpeaker] IN (1, 2);
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Speakers]', N'U') IS NULL
                BEGIN
                    EXEC sp_rename N'[Speaker]', N'Speakers';
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1 FROM sys.indexes
                       WHERE [name] = 'IX_Speaker_IdEvent'
                         AND [object_id] = OBJECT_ID(N'[Speakers]')
                   )
                BEGIN
                    EXEC sp_rename N'[Speakers].[IX_Speaker_IdEvent]', N'IX_Speakers_IdEvent', N'INDEX';
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND NOT EXISTS (
                       SELECT 1 FROM sys.key_constraints
                       WHERE [name] = 'PK_Speakers' AND [type] = 'PK'
                   )
                BEGIN
                    ALTER TABLE [Speakers] ADD CONSTRAINT [PK_Speakers] PRIMARY KEY ([IdSpeaker]);
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Session_has_Speaker]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Session_has_Speaker_Speakers_SpeakersIdSpeaker')
                BEGIN
                    ALTER TABLE [Session_has_Speaker]
                    ADD CONSTRAINT [FK_Session_has_Speaker_Speakers_SpeakersIdSpeaker]
                    FOREIGN KEY ([SpeakersIdSpeaker]) REFERENCES [Speakers] ([IdSpeaker]) ON DELETE CASCADE;
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Events]', N'U') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Speakers_Events_IdEvent')
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
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Session_has_Speaker_Speakers_SpeakersIdSpeaker')
                BEGIN
                    ALTER TABLE [Session_has_Speaker] DROP CONSTRAINT [FK_Session_has_Speaker_Speakers_SpeakersIdSpeaker];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Speakers_Events_IdEvent')
                BEGIN
                    ALTER TABLE [Speakers] DROP CONSTRAINT [FK_Speakers_Events_IdEvent];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1 FROM sys.key_constraints
                       WHERE [name] = 'PK_Speakers' AND [type] = 'PK'
                   )
                BEGIN
                    ALTER TABLE [Speakers] DROP CONSTRAINT [PK_Speakers];
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speakers]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Speaker]', N'U') IS NULL
                BEGIN
                    EXEC sp_rename N'[Speakers]', N'Speaker';
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1 FROM sys.indexes
                       WHERE [name] = 'IX_Speakers_IdEvent'
                         AND [object_id] = OBJECT_ID(N'[Speaker]')
                   )
                BEGIN
                    EXEC sp_rename N'[Speaker].[IX_Speakers_IdEvent]', N'IX_Speaker_IdEvent', N'INDEX';
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND NOT EXISTS (
                       SELECT 1 FROM sys.key_constraints
                       WHERE [name] = 'PK_Speaker' AND [type] = 'PK'
                   )
                BEGIN
                    ALTER TABLE [Speaker] ADD CONSTRAINT [PK_Speaker] PRIMARY KEY ([IdSpeaker]);
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                BEGIN
                    SET IDENTITY_INSERT [Speaker] ON;
                    INSERT INTO [Speaker] ([IdSpeaker], [Description], [FirstName], [IdEvent], [ImgPath], [LastName], [MiddleName])
                    VALUES
                        (1, NULL, N'Jan', 1, N'', N'Smit', NULL),
                        (2, NULL, N'John', 1, N'', N'Doe', NULL);
                    SET IDENTITY_INSERT [Speaker] OFF;
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Session_has_Speaker]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Session_has_Speaker_Speaker_SpeakersIdSpeaker')
                BEGIN
                    ALTER TABLE [Session_has_Speaker]
                    ADD CONSTRAINT [FK_Session_has_Speaker_Speaker_SpeakersIdSpeaker]
                    FOREIGN KEY ([SpeakersIdSpeaker]) REFERENCES [Speaker] ([IdSpeaker]) ON DELETE CASCADE;
                END
                """);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Speaker]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Events]', N'U') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Speaker_Events_IdEvent')
                BEGIN
                    ALTER TABLE [Speaker]
                    ADD CONSTRAINT [FK_Speaker_Events_IdEvent]
                    FOREIGN KEY ([IdEvent]) REFERENCES [Events] ([IdEvent]) ON DELETE NO ACTION;
                END
                """);
        }
    }
}
