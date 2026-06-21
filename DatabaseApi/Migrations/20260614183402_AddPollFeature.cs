using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPollFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    IdPoll = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IdSession = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.IdPoll);
                    table.ForeignKey(
                        name: "FK_Polls_Sessions_IdSession",
                        column: x => x.IdSession,
                        principalTable: "Sessions",
                        principalColumn: "IdSession",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PollAnswers",
                columns: table => new
                {
                    IdPollAnswer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdPoll = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollAnswers", x => x.IdPollAnswer);
                    table.ForeignKey(
                        name: "FK_PollAnswers_Polls_IdPoll",
                        column: x => x.IdPoll,
                        principalTable: "Polls",
                        principalColumn: "IdPoll",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollAnswers_IdPoll",
                table: "PollAnswers",
                column: "IdPoll");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_IdSession",
                table: "Polls",
                column: "IdSession");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollAnswers");

            migrationBuilder.DropTable(
                name: "Polls");
        }
    }
}
