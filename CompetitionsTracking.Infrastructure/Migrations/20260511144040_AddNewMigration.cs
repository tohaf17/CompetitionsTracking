using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_PersonId",
                table: "users",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_persons_PersonId",
                table: "users",
                column: "PersonId",
                principalTable: "persons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
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
