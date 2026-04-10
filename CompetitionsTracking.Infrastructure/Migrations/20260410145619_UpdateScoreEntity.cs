using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScoreEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BalanceCount",
                table: "scores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DynamicRotationCount",
                table: "scores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ElementCount",
                table: "scores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JumpCount",
                table: "scores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RotationCount",
                table: "scores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ControversialEntries",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    ParticipantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompetitionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestScore = table.Column<float>(type: "real", nullable: false),
                    LowestScore = table.Column<float>(type: "real", nullable: false),
                    ScoreGap = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "JudgeAnalytics",
                columns: table => new
                {
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    JudgeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPerformancesJudged = table.Column<int>(type: "int", nullable: false),
                    AverageScoreGiven = table.Column<float>(type: "real", nullable: false),
                    AverageScoreDeviation = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Leaderboards",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    ParticipantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisciplineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalScore = table.Column<float>(type: "real", nullable: false),
                    CalculatedRank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ParticipantPerformances",
                columns: table => new
                {
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    CompetitionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApparatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalScore = table.Column<float>(type: "real", nullable: false),
                    Placement = table.Column<int>(type: "int", nullable: false),
                    CompetitionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ScoreAnomalies",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "int", nullable: false),
                    JudgeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    ScoreValue = table.Column<float>(type: "real", nullable: false),
                    AverageEntryScore = table.Column<float>(type: "real", nullable: false),
                    Deviation = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TeamDominanceMetrics",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalParticipants = table.Column<int>(type: "int", nullable: false),
                    CumulativePoints = table.Column<float>(type: "real", nullable: false),
                    AveragePointsPerParticipant = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TeamMedalTallies",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoldMedals = table.Column<int>(type: "int", nullable: false),
                    SilverMedals = table.Column<int>(type: "int", nullable: false),
                    BronzeMedals = table.Column<int>(type: "int", nullable: false),
                    TotalMedals = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControversialEntries");

            migrationBuilder.DropTable(
                name: "JudgeAnalytics");

            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropTable(
                name: "ParticipantPerformances");

            migrationBuilder.DropTable(
                name: "ScoreAnomalies");

            migrationBuilder.DropTable(
                name: "TeamDominanceMetrics");

            migrationBuilder.DropTable(
                name: "TeamMedalTallies");

            migrationBuilder.DropColumn(
                name: "BalanceCount",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "DynamicRotationCount",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "ElementCount",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "JumpCount",
                table: "scores");

            migrationBuilder.DropColumn(
                name: "RotationCount",
                table: "scores");
        }
    }
}
