using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_has_Tag_Tags_TagsTitle_TagsIdEvent",
                table: "Session_has_Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Session_has_Tag",
                table: "Session_has_Tag");

            migrationBuilder.DropIndex(
                name: "IX_Session_has_Tag_TagsTitle_TagsIdEvent",
                table: "Session_has_Tag");

            migrationBuilder.DropColumn(
                name: "TagsTitle",
                table: "Session_has_Tag");

            migrationBuilder.RenameColumn(
                name: "TagsIdEvent",
                table: "Session_has_Tag",
                newName: "TagsIdTag");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "IdTag",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "IdTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Session_has_Tag",
                table: "Session_has_Tag",
                columns: new[] { "SessionsIdSession", "TagsIdTag" });


            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Tag_TagsIdTag",
                table: "Session_has_Tag",
                column: "TagsIdTag");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_has_Tag_Tags_TagsIdTag",
                table: "Session_has_Tag",
                column: "TagsIdTag",
                principalTable: "Tags",
                principalColumn: "IdTag",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_has_Tag_Tags_TagsIdTag",
                table: "Session_has_Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Session_has_Tag",
                table: "Session_has_Tag");

            migrationBuilder.DropIndex(
                name: "IX_Session_has_Tag_TagsIdTag",
                table: "Session_has_Tag");

            migrationBuilder.DropColumn(
                name: "IdTag",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "TagsIdTag",
                table: "Session_has_Tag",
                newName: "TagsIdEvent");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TagsTitle",
                table: "Session_has_Tag",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                columns: new[] { "Title", "IdEvent" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Session_has_Tag",
                table: "Session_has_Tag",
                columns: new[] { "SessionsIdSession", "TagsTitle", "TagsIdEvent" });

            migrationBuilder.CreateIndex(
                name: "IX_Session_has_Tag_TagsTitle_TagsIdEvent",
                table: "Session_has_Tag",
                columns: new[] { "TagsTitle", "TagsIdEvent" });

            migrationBuilder.AddForeignKey(
                name: "FK_Session_has_Tag_Tags_TagsTitle_TagsIdEvent",
                table: "Session_has_Tag",
                columns: new[] { "TagsTitle", "TagsIdEvent" },
                principalTable: "Tags",
                principalColumns: new[] { "Title", "IdEvent" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
