using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Staging__AddUserDataFromMoodle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Staging)
            {
                migrationBuilder.Sql(
                    """
                    INSERT INTO academic_group(id, name)
                    VALUES (gen_random_uuid(), 'ПрИ-404')
                    ON CONFLICT DO NOTHING;

                    INSERT INTO student(id, full_name, group_id)
                    VALUES (gen_random_uuid(), 'Иванов Иван', (SELECT id FROM academic_group)),
                           (gen_random_uuid(), 'Петров Петр', (SELECT id FROM academic_group))
                    ON CONFLICT DO NOTHING;

                    INSERT INTO moodle_account(id, ext_id, username, student_id)
                    VALUES (gen_random_uuid(), 3, 'user1', (SELECT id FROM student WHERE full_name = 'Иванов Иван')),
                           (gen_random_uuid(), 4, 'user2', (SELECT id FROM student WHERE full_name = 'Петров Петр'))
                    ON CONFLICT DO NOTHING;
                    """
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Staging)
            {
                migrationBuilder.Sql(
                    """
                    DELETE FROM moodle_account
                    WHERE ext_id IN (3, 4);

                    DELETE FROM student
                    WHERE group_id = (SELECT id 
                                      FROM academic_group
                                      WHERE name = 'ПрИ-404');

                    DELETE FROM academic_group
                    WHERE name = 'ПрИ-404';
                    """
                );
            }
        }
    }
}
