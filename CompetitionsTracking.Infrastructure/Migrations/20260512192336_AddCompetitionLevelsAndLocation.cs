using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionLevelsAndLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "competitions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "competitions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "competitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "National");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "competitions");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "competitions");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "competitions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
