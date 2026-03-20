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
                    idEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mainColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    accentColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    logoPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.idEvent);
                });

            migrationBuilder.CreateTable(
                name: "Speakers",
                columns: table => new
                {
                    idSpeaker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imgPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.idSpeaker);
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
                    idRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roomLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.idRoom);
                    table.ForeignKey(
                        name: "FK_Rooms_Events_idEvent",
                        column: x => x.idEvent,
                        principalTable: "Events",
                        principalColumn: "idEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    idEvent = table.Column<int>(type: "int", nullable: false),
                    colorHex = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.title, x.idEvent });
                    table.ForeignKey(
                        name: "FK_Tags_Events_idEvent",
                        column: x => x.idEvent,
                        principalTable: "Events",
                        principalColumn: "idEvent",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_has_Event",
                columns: table => new
                {
                    EventsidEvent = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_has_Event", x => new { x.EventsidEvent, x.UsersId });
                    table.ForeignKey(
                        name: "FK_User_has_Event_Events_EventsidEvent",
                        column: x => x.EventsidEvent,
                        principalTable: "Events",
                        principalColumn: "idEvent",
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
                    idSession = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    plenary = table.Column<bool>(type: "bit", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    idEvent = table.Column<int>(type: "int", nullable: false),
                    idRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.idSession);
                    table.ForeignKey(
                        name: "FK_Sessions_Events_idEvent",
                        column: x => x.idEvent,
                        principalTable: "Events",
                        principalColumn: "idEvent",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Rooms_idRoom",
                        column: x => x.idRoom,
                        principalTable: "Rooms",
                        principalColumn: "idRoom",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Session_has_Speaker",
                columns: table => new
                {
                    SessionsidSession = table.Column<int>(type: "int", nullable: false),
                    SpeakersidSpeaker = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session_has_Speaker", x => new { x.SessionsidSession, x.SpeakersidSpeaker });
                    table.ForeignKey(
                        name: "FK_Session_has_Speaker_Sessions_SessionsidSession",
                        column: x => x.SessionsidSession,
                        principalTable: "Sessions",
                        principalColumn: "idSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Session_has_Speaker_Speakers_SpeakersidSpeaker",
                        column: x => x.SpeakersidSpeaker,
                        principalTable: "Speakers",
                        principalColumn: "idSpeaker",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Session_has_Tag",
                columns: table => new
                {
                    SessionsidSession = table.Column<int>(type: "int", nullable: false),
                    Tagstitle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagsidEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session_has_Tag", x => new { x.SessionsidSession, x.Tagstitle, x.TagsidEvent });
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Sessions_SessionsidSession",
                        column: x => x.SessionsidSession,
                        principalTable: "Sessions",
                        principalColumn: "idSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Tags_Tagstitle_TagsidEvent",
                        columns: x => new { x.Tagstitle, x.TagsidEvent },
                        principalTable: "Tags",
                        principalColumns: new[] { "title", "idEvent" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_has_Sessions",
                columns: table => new
                {
                    idUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    idSession = table.Column<int>(type: "int", nullable: false),
                    inWaitingList = table.Column<bool>(type: "bit", nullable: false),
                    joinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_has_Sessions", x => new { x.idUser, x.idSession });
                    table.ForeignKey(
                        name: "FK_User_has_Sessions_Sessions_idSession",
                        column: x => x.idSession,
                        principalTable: "Sessions",
                        principalColumn: "idSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_has_Sessions_Users_idUser",
                        column: x => x.idUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_idEvent",
                table: "Rooms",
                column: "idEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Speaker_SpeakersidSpeaker",
                table: "Session_has_Speaker",
                column: "SpeakersidSpeaker");

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Tag_Tagstitle_TagsidEvent",
                table: "Session_has_Tag",
                columns: new[] { "Tagstitle", "TagsidEvent" });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_idEvent",
                table: "Sessions",
                column: "idEvent");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_idRoom",
                table: "Sessions",
                column: "idRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_idEvent",
                table: "Tags",
                column: "idEvent");

            migrationBuilder.CreateIndex(
                name: "IX_User_has_Event_UsersId",
                table: "User_has_Event",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_User_has_Sessions_idSession",
                table: "User_has_Sessions",
                column: "idSession");
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
