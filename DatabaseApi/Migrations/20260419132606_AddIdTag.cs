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

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                keyColumnTypes: new[] { "int", "int", "nvarchar(450)" },
                keyValues: new object[] { 1, 1, "Plenaire sessie" });

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                keyColumnTypes: new[] { "int", "int", "nvarchar(450)" },
                keyValues: new object[] { 1, 1, "Technology" });

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumns: new[] { "IdEvent", "Title" },
                keyValues: new object[] { 1, "Plenaire sessie" });

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumns: new[] { "IdEvent", "Title" },
                keyValues: new object[] { 1, "Technology" });

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

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "IdTag", "ColorHex", "IdEvent", "Title" },
                values: new object[,]
                {
                    { 1, "#D5B82C", 1, "Plenaire sessie" },
                    { 2, "#2CCFD5", 1, "Technology" }
                });

            migrationBuilder.InsertData(
                table: "Session_has_Tag",
                columns: new[] { "SessionsIdSession", "TagsIdTag" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 }
                });

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

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdTag" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Session_has_Tag",
                keyColumns: new[] { "SessionsIdSession", "TagsIdTag" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "IdTag",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "IdTag",
                keyColumnType: "int",
                keyValue: 2);

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

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "IdEvent", "Title", "ColorHex" },
                values: new object[,]
                {
                    { 1, "Plenaire sessie", "#D5B82C" },
                    { 1, "Technology", "#2CCFD5" }
                });

            migrationBuilder.InsertData(
                table: "Session_has_Tag",
                columns: new[] { "SessionsIdSession", "TagsIdEvent", "TagsTitle" },
                values: new object[,]
                {
                    { 1, 1, "Plenaire sessie" },
                    { 1, 1, "Technology" }
                });

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
