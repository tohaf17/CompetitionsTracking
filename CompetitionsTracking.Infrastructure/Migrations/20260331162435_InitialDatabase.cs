using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apparatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apparatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    MaxAge = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "competitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApparatusId = table.Column<int>(type: "int", nullable: false),
                    ApparatusId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disciplines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_disciplines_apparatus_ApparatusId",
                        column: x => x.ApparatusId,
                        principalTable: "apparatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_disciplines_apparatus_ApparatusId1",
                        column: x => x.ApparatusId1,
                        principalTable: "apparatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MentorId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persons_participants_Id",
                        column: x => x.Id,
                        principalTable: "participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persons_persons_MentorId",
                        column: x => x.MentorId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    DisciplineId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId1 = table.Column<int>(type: "int", nullable: true),
                    CompetitionId1 = table.Column<int>(type: "int", nullable: true),
                    DisciplineId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entries_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entries_categories_CategoryId1",
                        column: x => x.CategoryId1,
                        principalTable: "categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_entries_competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entries_competitions_CompetitionId1",
                        column: x => x.CompetitionId1,
                        principalTable: "competitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_entries_disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entries_disciplines_DisciplineId1",
                        column: x => x.DisciplineId1,
                        principalTable: "disciplines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_entries_participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "judges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    QualificationLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_judges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_judges_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoachId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_participants_Id",
                        column: x => x.Id,
                        principalTable: "participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_persons_CoachId",
                        column: x => x.CoachId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    Place = table.Column<int>(type: "int", nullable: false),
                    FinalScore = table.Column<float>(type: "real", nullable: false),
                    AwardedMedal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_results_entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    JudgeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScoreValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scores_entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scores_judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "int", nullable: false),
                    person_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_members", x => new { x.team_id, x.person_id });
                    table.ForeignKey(
                        name: "FK_team_members_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_members_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResultId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appeals_results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appeals_results_ResultId1",
                        column: x => x.ResultId1,
                        principalTable: "results",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appeals_ResultId",
                table: "appeals",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_appeals_ResultId1",
                table: "appeals",
                column: "ResultId1");

            migrationBuilder.CreateIndex(
                name: "IX_disciplines_ApparatusId",
                table: "disciplines",
                column: "ApparatusId");

            migrationBuilder.CreateIndex(
                name: "IX_disciplines_ApparatusId1",
                table: "disciplines",
                column: "ApparatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_entries_CategoryId",
                table: "entries",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_entries_CategoryId1",
                table: "entries",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_entries_CompetitionId_ParticipantId_DisciplineId",
                table: "entries",
                columns: new[] { "CompetitionId", "ParticipantId", "DisciplineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_entries_CompetitionId1",
                table: "entries",
                column: "CompetitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_entries_DisciplineId",
                table: "entries",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_entries_DisciplineId1",
                table: "entries",
                column: "DisciplineId1");

            migrationBuilder.CreateIndex(
                name: "IX_entries_ParticipantId",
                table: "entries",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_judges_PersonId",
                table: "judges",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_persons_MentorId",
                table: "persons",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_results_EntryId",
                table: "results",
                column: "EntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scores_EntryId",
                table: "scores",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_scores_JudgeId",
                table: "scores",
                column: "JudgeId");

            migrationBuilder.CreateIndex(
                name: "IX_team_members_person_id",
                table: "team_members",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_members_team_id_person_id",
                table: "team_members",
                columns: new[] { "team_id", "person_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_CoachId",
                table: "teams",
                column: "CoachId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appeals");

            migrationBuilder.DropTable(
                name: "scores");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "judges");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "entries");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "competitions");

            migrationBuilder.DropTable(
                name: "disciplines");

            migrationBuilder.DropTable(
                name: "participants");

            migrationBuilder.DropTable(
                name: "apparatus");
        }
    }
}
