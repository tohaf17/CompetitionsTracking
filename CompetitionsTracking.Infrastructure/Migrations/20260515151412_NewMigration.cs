using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Medalists",
                table: "TeamMedalTallies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Medalists",
                table: "TeamMedalTallies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
