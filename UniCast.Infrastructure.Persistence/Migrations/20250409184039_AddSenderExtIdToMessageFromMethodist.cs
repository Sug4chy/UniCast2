using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderExtIdToMessageFromMethodist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sender_ext_id",
                table: "message_from_methodist",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sender_ext_id",
                table: "message_from_methodist");
        }
    }
}
