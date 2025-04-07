using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMoodleAccountEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "moodle_account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ext_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    current_token = table.Column<string>(type: "text", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moodle_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_moodle_account_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_moodle_account_ext_id",
                table: "moodle_account",
                column: "ext_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_moodle_account_student_id",
                table: "moodle_account",
                column: "student_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_moodle_account_username",
                table: "moodle_account",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moodle_account");
        }
    }
}
