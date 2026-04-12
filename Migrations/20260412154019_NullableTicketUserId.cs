using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSupportSystem.Migrations
{
    /// <inheritdoc />
    public partial class NullableTicketUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tickets_TicketId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tickets_TicketId",
                table: "Attachments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tickets_TicketId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tickets_TicketId",
                table: "Attachments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
