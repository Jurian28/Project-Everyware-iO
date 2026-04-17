using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccentColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.IdEvent);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Speakers",
                columns: table => new
                {
                    IdSpeaker = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speakers", x => x.IdSpeaker);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomLabel = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    IdTag = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdEvent = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.IdTag);
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
                        name: "FK_User_has_Event_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_has_Event_Events_EventsIdEvent",
                        column: x => x.EventsIdEvent,
                        principalTable: "Events",
                        principalColumn: "IdEvent",
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
                    Capacity = table.Column<int>(type: "int", nullable: true),
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
                    TagsIdTag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session_has_Tag", x => new { x.SessionsIdSession, x.TagsIdTag });
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Sessions_SessionsIdSession",
                        column: x => x.SessionsIdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Session_has_Tag_Tags_TagsIdTag",
                        column: x => x.TagsIdTag,
                        principalTable: "Tags",
                        principalColumn: "IdTag",
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
                        name: "FK_User_has_Sessions_AspNetUsers_IdUser",
                        column: x => x.IdUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_has_Sessions_Sessions_IdSession",
                        column: x => x.IdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Cascade);
                });

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
                columns: new[] { "IdTag", "ColorHex", "IdEvent", "Title" },
                values: new object[,]
                {
                    { 1, "#D5B82C", 1, "Plenaire sessie" },
                    { 2, "#2CCFD5", 1, "Technology" }
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
                columns: new[] { "SessionsIdSession", "TagsIdTag" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 }
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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdEvent_RoomLabel",
                table: "Rooms",
                columns: new[] { "IdEvent", "RoomLabel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Speaker_SpeakersIdSpeaker",
                table: "Session_has_Speaker",
                column: "SpeakersIdSpeaker");

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Tag_TagsIdTag",
                table: "Session_has_Tag",
                column: "TagsIdTag");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Session_has_Speaker");

            migrationBuilder.DropTable(
                name: "Session_has_Tag");

            migrationBuilder.DropTable(
                name: "User_has_Event");

            migrationBuilder.DropTable(
                name: "User_has_Sessions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Speakers");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
