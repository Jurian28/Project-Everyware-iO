using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Speakers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImgPath",
                table: "Speakers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Speakers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MainColorHex",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LogoPath",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AccentColorHex",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, null, "jan.smit@example.com", true, false, null, "JAN.SMIT@EXAMPLE.COM", "JAN.SMIT@EXAMPLE.COM", "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==", null, false, "STATIC-STAMP-001", false, "jan.smit@example.com" },
                    { "2", 0, null, "john.doe@example.com", true, false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE@EXAMPLE.COM", "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==", null, false, "STATIC-STAMP-002", false, "john.doe@example.com" },
                    { "3", 0, null, "jane.smith@example.com", true, false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH@EXAMPLE.COM", "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==", null, false, "STATIC-STAMP-003", false, "jane.smith@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "IdEvent", "AccentColorHex", "Description", "EndDate", "LogoPath", "MainColorHex", "StartDate", "Title" },
                values: new object[] { 1, "#B0B0B0", "Hier zal besproken worden wat er allemaal gemaakt moet worden voor de beste event calender ooit.", new DateTime(2026, 10, 10, 17, 0, 0, 0, DateTimeKind.Unspecified), "", "#D9D9D9", new DateTime(2026, 10, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "iO Event Connect" });

            migrationBuilder.InsertData(
                table: "Speakers",
                columns: new[] { "IdSpeaker", "FirstName", "ImgPath", "LastName", "MiddleName", "description" },
                values: new object[,]
                {
                    { 1, "Jan", "", "Smit", null, "Expert in C# en Cloud." },
                    { 2, "John", "", "Doe", null, "Expert in Databases en networking." }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "Description", "IdEvent", "RoomLabel" },
                values: new object[,]
                {
                    { 1, 100, "Grote conferentiezaal.", 1, "Hoofdzaal" },
                    { 2, 20, "Grote meeting zaal.", 1, "Kamer 1" },
                    { 3, 2, "Kleine meeting zaal.", 1, "Kamer 2" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "IdEvent", "Title", "ColorHex" },
                values: new object[,]
                {
                    { 1, "Plenaire sessie", "#D5B82C" },
                    { 1, "Technology", "#2CCFD5" }
                });

            migrationBuilder.InsertData(
                table: "User_has_Event",
                columns: new[] { "EventsIdEvent", "UsersId" },
                values: new object[,]
                {
                    { 1, "1" },
                    { 1, "2" },
                    { 1, "3" }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "IdSession", "Capacity", "EndTime", "IdEvent", "IdRoom", "Plenary", "StartTime", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 10, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, true, new DateTime(2026, 10, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Bespreking algemene eisen en wensen." },
                    { 2, 20, new DateTime(2026, 10, 10, 11, 30, 0, 0, DateTimeKind.Unspecified), 1, 2, false, new DateTime(2026, 10, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), "Database architectuur." },
                    { 3, 2, new DateTime(2026, 10, 10, 11, 30, 0, 0, DateTimeKind.Unspecified), 1, 3, false, new DateTime(2026, 10, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), "Routing architectuur." }
                });

            migrationBuilder.InsertData(
                table: "Session_has_Speaker",
                columns: new[] { "SessionsIdSession", "SpeakersIdSpeaker" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Session_has_Tag",
                columns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                values: new object[,]
                {
                    { 1, 1, "Plenaire sessie" },
                    { 1, 1, "Technology" }
                });

            migrationBuilder.InsertData(
                table: "User_has_Sessions",
                columns: new[] { "IdSession", "IdUser", "InWaitingList", "JoinedDate" },
                values: new object[,]
                {
                    { 1, "1", false, new DateTime(2026, 10, 10, 8, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "1", false, new DateTime(2026, 10, 10, 8, 5, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "2", false, new DateTime(2026, 10, 10, 10, 10, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Session_has_Speaker",
                keyColumns: new[] { "SessionsIdSession", "SpeakersIdSpeaker" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                keyValues: new object[] { 1, 1, "Plenaire sessie" });

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                keyValues: new object[] { 1, 1, "Technology" });

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User_has_Event",
                keyColumns: new[] { "EventsIdEvent", "UsersId" },
                keyValues: new object[] { 1, "1" });

            migrationBuilder.DeleteData(
                table: "User_has_Event",
                keyColumns: new[] { "EventsIdEvent", "UsersId" },
                keyValues: new object[] { 1, "2" });

            migrationBuilder.DeleteData(
                table: "User_has_Event",
                keyColumns: new[] { "EventsIdEvent", "UsersId" },
                keyValues: new object[] { 1, "3" });

            migrationBuilder.DeleteData(
                table: "User_has_Sessions",
                keyColumns: new[] { "IdSession", "IdUser" },
                keyValues: new object[] { 1, "1" });

            migrationBuilder.DeleteData(
                table: "User_has_Sessions",
                keyColumns: new[] { "IdSession", "IdUser" },
                keyValues: new object[] { 3, "1" });

            migrationBuilder.DeleteData(
                table: "User_has_Sessions",
                keyColumns: new[] { "IdSession", "IdUser" },
                keyValues: new object[] { 3, "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "IdSession",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Speakers",
                keyColumn: "IdSpeaker",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumns: new[] { "IdEvent", "Title" },
                keyValues: new object[] { 1, "Plenaire sessie" });

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumns: new[] { "IdEvent", "Title" },
                keyValues: new object[] { 1, "Technology" });

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "IdEvent",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Speakers");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Speakers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImgPath",
                table: "Speakers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MainColorHex",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoPath",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccentColorHex",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
