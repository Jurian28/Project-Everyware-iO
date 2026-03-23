using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccentColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.IdEvent);
                });

            migrationBuilder.CreateTable(
                name: "Speakers",
                columns: table => new
                {
                    IdSpeaker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.IdSpeaker);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.IdRoom);
                    table.ForeignKey(
                        name: "FK_Rooms_Events_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.Title, x.IdEvent });
                    table.ForeignKey(
                        name: "FK_Tags_Events_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_has_Event",
                columns: table => new
                {
                    EventsIdEvent = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_has_Event", x => new { x.EventsIdEvent, x.UsersId });
                    table.ForeignKey(
                        name: "FK_User_has_Event_Events_EventsIdEvent",
                        column: x => x.EventsIdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_has_Event_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    IdSession = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Plenary = table.Column<bool>(type: "bit", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.IdSession);
                    table.ForeignKey(
                        name: "FK_Sessions_Events_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Rooms_IdRoom",
                        column: x => x.IdRoom,
                        principalTable: "Rooms",
                        principalColumn: "IdRoom",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Session_has_Speaker",
                columns: table => new
                {
                    SessionsIdSession = table.Column<int>(type: "int", nullable: false),
                    SpeakersIdSpeaker = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session_has_Speaker", x => new { x.SessionsIdSession, x.SpeakersIdSpeaker });
                    table.ForeignKey(
                        name: "FK_Session_has_Speaker_Sessions_SessionsIdSession",
                        column: x => x.SessionsIdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Session_has_Speaker_Speakers_SpeakersIdSpeaker",
                        column: x => x.SpeakersIdSpeaker,
                        principalTable: "Speakers",
                        principalColumn: "IdSpeaker",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Session_has_Tag",
                columns: table => new
                {
                    SessionsIdSession = table.Column<int>(type: "int", nullable: false),
                    TagsTitle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagsIdEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session_has_Tag", x => new { x.SessionsIdSession, x.TagsTitle, x.TagsIdEvent });
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Sessions_SessionsIdSession",
                        column: x => x.SessionsIdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Tags_TagsTitle_TagsIdEvent",
                        columns: x => new { x.TagsTitle, x.TagsIdEvent },
                        principalTable: "Tags",
                        principalColumns: new[] { "Title", "IdEvent" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_has_Sessions",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdSession = table.Column<int>(type: "int", nullable: false),
                    InWaitingList = table.Column<bool>(type: "bit", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_has_Sessions", x => new { x.IdUser, x.IdSession });
                    table.ForeignKey(
                        name: "FK_User_has_Sessions_Sessions_IdSession",
                        column: x => x.IdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_has_Sessions_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdEvent",
                table: "Rooms",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Speaker_SpeakersIdSpeaker",
                table: "Session_has_Speaker",
                column: "SpeakersIdSpeaker");

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Tag_TagsTitle_TagsIdEvent",
                table: "Session_has_Tag",
                columns: new[] { "TagsTitle", "TagsIdEvent" });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdEvent",
                table: "Sessions",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdRoom",
                table: "Sessions",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IdEvent",
                table: "Tags",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_User_has_Event_UsersId",
                table: "User_has_Event",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_User_has_Sessions_IdSession",
                table: "User_has_Sessions",
                column: "IdSession");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Session_has_Speaker");

            migrationBuilder.DropTable(
                name: "Session_has_Tag");

            migrationBuilder.DropTable(
                name: "User_has_Event");

            migrationBuilder.DropTable(
                name: "User_has_Sessions");

            migrationBuilder.DropTable(
                name: "Speakers");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
