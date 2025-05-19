using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTelegramMessageReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telegram_message_reaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telegram_message_reaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reaction = table.Column<string>(type: "text", nullable: false),
                    reactor_username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telegram_message_reaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_telegram_message_reaction_telegram_message_message_id",
                        column: x => x.message_id,
                        principalTable: "telegram_message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_telegram_message_reaction_message_id",
                table: "telegram_message_reaction",
                column: "message_id");
        }
    }
}
