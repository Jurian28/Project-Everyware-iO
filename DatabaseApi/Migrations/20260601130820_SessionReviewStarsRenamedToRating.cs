using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class SessionReviewStarsRenamedToRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "comment",
                table: "SessionReviews",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "stars",
                table: "SessionReviews",
                newName: "Rating");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "SessionReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "SessionReviews",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "SessionReviews",
                newName: "stars");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "SessionReviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
