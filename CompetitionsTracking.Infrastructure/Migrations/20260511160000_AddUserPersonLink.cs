using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    public partial class AddUserPersonLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE u
                SET PersonId = c.Id
                FROM users u
                CROSS APPLY (
                    SELECT TOP 1 p.Id
                    FROM persons p
                    WHERE EXISTS (SELECT 1 FROM persons m WHERE m.MentorId = p.Id)
                       OR EXISTS (SELECT 1 FROM teams t WHERE t.CoachId = p.Id)
                    ORDER BY p.Id
                ) c
                WHERE u.Username = 'trainee' AND u.PersonId IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_users_PersonId",
                table: "users",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_persons_PersonId",
                table: "users",
                column: "PersonId",
                principalTable: "persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_persons_PersonId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_PersonId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "users");
        }
    }
}
