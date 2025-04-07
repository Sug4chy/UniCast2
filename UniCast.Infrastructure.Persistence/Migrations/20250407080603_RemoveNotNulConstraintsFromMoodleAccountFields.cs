using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotNulConstraintsFromMoodleAccountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_moodle_account_student_student_id",
                table: "moodle_account");

            migrationBuilder.AlterColumn<Guid>(
                name: "student_id",
                table: "moodle_account",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "current_token",
                table: "moodle_account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_moodle_account_student_student_id",
                table: "moodle_account",
                column: "student_id",
                principalTable: "student",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_moodle_account_student_student_id",
                table: "moodle_account");

            migrationBuilder.AlterColumn<Guid>(
                name: "student_id",
                table: "moodle_account",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "current_token",
                table: "moodle_account",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_moodle_account_student_student_id",
                table: "moodle_account",
                column: "student_id",
                principalTable: "student",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
