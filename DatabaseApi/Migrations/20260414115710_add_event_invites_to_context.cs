using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseApi.Migrations
{
    /// <inheritdoc />
    public partial class add_event_invites_to_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventInvite_Events_EventIdEvent",
                table: "EventInvite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventInvite",
                table: "EventInvite");

            migrationBuilder.RenameTable(
                name: "EventInvite",
                newName: "EventInvites");

            migrationBuilder.RenameIndex(
                name: "IX_EventInvite_EventIdEvent",
                table: "EventInvites",
                newName: "IX_EventInvites_EventIdEvent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventInvites",
                table: "EventInvites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvites_Events_EventIdEvent",
                table: "EventInvites",
                column: "EventIdEvent",
                principalTable: "Events",
                principalColumn: "IdEvent",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventInvites_Events_EventIdEvent",
                table: "EventInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventInvites",
                table: "EventInvites");

            migrationBuilder.RenameTable(
                name: "EventInvites",
                newName: "EventInvite");

            migrationBuilder.RenameIndex(
                name: "IX_EventInvites_EventIdEvent",
                table: "EventInvite",
                newName: "IX_EventInvite_EventIdEvent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventInvite",
                table: "EventInvite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvite_Events_EventIdEvent",
                table: "EventInvite",
                column: "EventIdEvent",
                principalTable: "Events",
                principalColumn: "IdEvent",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
