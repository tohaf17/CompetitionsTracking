using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CompetitionsTracking.Domain.Entities;
using CompetitionsTracking.Domain.Models;

namespace CompetitionsTracking.Infrastructure.Data
{
    public class CompetitionsTrackingDbContext : DbContext
    {
        public CompetitionsTrackingDbContext(DbContextOptions<CompetitionsTrackingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Participant> Participants { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Apparatus> Apparatuses { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Appeal> Appeals { get; set; }
        public DbSet<Judge> Judges { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<LeaderboardDto> Leaderboards { get; set; }
        public DbSet<JudgeAnalyticsDto> JudgeAnalytics { get; set; }
        public DbSet<ParticipantPerformanceDto> ParticipantPerformances { get; set; }
        public DbSet<ControversialEntryDto> ControversialEntries { get; set; }
        public DbSet<TeamMedalTallyDto> TeamMedalTallies { get; set; }
        public DbSet<TeamDominanceMetricDto> TeamDominanceMetrics { get; set; }
        public DbSet<ScoreAnomalyDto> ScoreAnomalies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LeaderboardDto>().HasNoKey();
            modelBuilder.Entity<JudgeAnalyticsDto>().HasNoKey();
            modelBuilder.Entity<ParticipantPerformanceDto>().HasNoKey();
            modelBuilder.Entity<ControversialEntryDto>().HasNoKey();
            modelBuilder.Entity<TeamMedalTallyDto>().HasNoKey();
            modelBuilder.Entity<TeamDominanceMetricDto>().HasNoKey();
            modelBuilder.Entity<ScoreAnomalyDto>().HasNoKey();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompetitionsTrackingDbContext).Assembly);
        }
    }
}