using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPollVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    IdPollVote = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdPoll = table.Column<int>(type: "int", nullable: false),
                    IdPollAnswer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVotes", x => x.IdPollVote);
                    table.ForeignKey(
                        name: "FK_PollVotes_PollAnswers_IdPollAnswer",
                        column: x => x.IdPollAnswer,
                        principalTable: "PollAnswers",
                        principalColumn: "IdPollAnswer",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_IdPollAnswer",
                table: "PollVotes",
                column: "IdPollAnswer");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_IdUser_IdPoll",
                table: "PollVotes",
                columns: new[] { "IdUser", "IdPoll" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollVotes");
        }
    }
}
