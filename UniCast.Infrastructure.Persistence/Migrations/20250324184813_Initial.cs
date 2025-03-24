using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "message_from_methodist",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    sender_username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_from_methodist", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student", x => x.id);
                    table.ForeignKey(
                        name: "FK_student_academic_group_group_id",
                        column: x => x.group_id,
                        principalTable: "academic_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_from_methodist_student",
                columns: table => new
                {
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_from_methodist_student", x => new { x.student_id, x.message_id });
                    table.ForeignKey(
                        name: "FK_message_from_methodist_student_message_from_methodist_messa~",
                        column: x => x.message_id,
                        principalTable: "message_from_methodist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_message_from_methodist_student_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "students_reply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reply_text = table.Column<string>(type: "text", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students_reply", x => x.id);
                    table.ForeignKey(
                        name: "FK_students_reply_message_from_methodist_message_id",
                        column: x => x.message_id,
                        principalTable: "message_from_methodist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_students_reply_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_chat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    ext_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: true),
                    current_scenario = table.Column<int>(type: "integer", nullable: true),
                    current_state = table.Column<int>(type: "integer", nullable: true),
                    group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    current_scenario_args = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telegram_chat", x => x.id);
                    table.ForeignKey(
                        name: "FK_telegram_chat_academic_group_group_id",
                        column: x => x.group_id,
                        principalTable: "academic_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_telegram_chat_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "telegram_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ext_id = table.Column<int>(type: "integer", nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    src_message_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telegram_message", x => x.id);
                    table.ForeignKey(
                        name: "FK_telegram_message_message_from_methodist_src_message_id",
                        column: x => x.src_message_id,
                        principalTable: "message_from_methodist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_telegram_message_telegram_chat_chat_id",
                        column: x => x.chat_id,
                        principalTable: "telegram_chat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telegram_message_reaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reactor_username = table.Column<string>(type: "text", nullable: false),
                    reaction = table.Column<string>(type: "text", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "IX_academic_group_name",
                table: "academic_group",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_from_methodist_student_message_id",
                table: "message_from_methodist_student",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_group_id",
                table: "student",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_students_reply_message_id",
                table: "students_reply",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_students_reply_student_id",
                table: "students_reply",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_telegram_chat_ext_id",
                table: "telegram_chat",
                column: "ext_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telegram_chat_group_id",
                table: "telegram_chat",
                column: "group_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telegram_chat_student_id",
                table: "telegram_chat",
                column: "student_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telegram_chat_title",
                table: "telegram_chat",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telegram_message_chat_id",
                table: "telegram_message",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_telegram_message_ext_id_chat_id",
                table: "telegram_message",
                columns: new[] { "ext_id", "chat_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telegram_message_src_message_id",
                table: "telegram_message",
                column: "src_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_telegram_message_reaction_message_id",
                table: "telegram_message_reaction",
                column: "message_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_from_methodist_student");

            migrationBuilder.DropTable(
                name: "students_reply");

            migrationBuilder.DropTable(
                name: "telegram_message_reaction");

            migrationBuilder.DropTable(
                name: "telegram_message");

            migrationBuilder.DropTable(
                name: "message_from_methodist");

            migrationBuilder.DropTable(
                name: "telegram_chat");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "academic_group");
        }
    }
}
