using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Staging__AddUser3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Staging)
            {
                migrationBuilder.Sql(
                    """
                    INSERT INTO student(id, full_name, group_id)
                    VALUES (gen_random_uuid(), 'Антонов Антон', (SELECT id FROM academic_group))
                    ON CONFLICT DO NOTHING;

                    INSERT INTO moodle_account(id, ext_id, username, student_id) 
                    VALUES (gen_random_uuid(), 7, 'user3', (SELECT id FROM student WHERE full_name = 'Антонов Антон'))
                    ON CONFLICT DO NOTHING;
                    """
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
