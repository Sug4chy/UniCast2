using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAcademicGroupsAndTelegramChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_academic_group_group_id",
                table: "student");

            migrationBuilder.DropForeignKey(
                name: "FK_telegram_chat_academic_group_group_id",
                table: "telegram_chat");

            migrationBuilder.DropTable(
                name: "academic_group");

            migrationBuilder.DropIndex(
                name: "IX_telegram_chat_group_id",
                table: "telegram_chat");

            migrationBuilder.DropIndex(
                name: "IX_student_group_id",
                table: "student");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "telegram_chat");

            migrationBuilder.DropColumn(
                name: "type",
                table: "telegram_chat");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "student");

            migrationBuilder.AlterColumn<string>(
                name: "current_scenario_args",
                table: "telegram_chat",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "current_scenario_args",
                table: "telegram_chat",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<Guid>(
                name: "group_id",
                table: "telegram_chat",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "type",
                table: "telegram_chat",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<Guid>(
                name: "group_id",
                table: "student",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "academic_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_group", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_telegram_chat_group_id",
                table: "telegram_chat",
                column: "group_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_group_id",
                table: "student",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_academic_group_name",
                table: "academic_group",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_student_academic_group_group_id",
                table: "student",
                column: "group_id",
                principalTable: "academic_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_telegram_chat_academic_group_group_id",
                table: "telegram_chat",
                column: "group_id",
                principalTable: "academic_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
