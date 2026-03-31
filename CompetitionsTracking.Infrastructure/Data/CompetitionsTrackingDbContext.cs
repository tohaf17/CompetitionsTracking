using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Infrastructure.Data
{
    public class CompetitionsTrackingDbContext : DbContext
    {
        public CompetitionsTrackingDbContext(DbContextOptions<CompetitionsTrackingDbContext> options) : base(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Participant>()
                .UseTptMappingStrategy()
                .ToTable("participants");

            modelBuilder.Entity<Person>()
                .ToTable("persons");

            modelBuilder.Entity<Team>()
                .ToTable("teams");

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Mentor)
                .WithMany(p => p.Mentees)
                .HasForeignKey(p => p.MentorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Coach)
                .WithMany()
                .HasForeignKey(t => t.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Members)
                .WithMany(p => p.Teams)
                .UsingEntity<Dictionary<string, object>>(
                    "team_members",
                    j => j
                        .HasOne<Person>()
                        .WithMany()
                        .HasForeignKey("person_id")
                        .HasConstraintName("FK_team_members_persons_person_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Team>()
                        .WithMany()
                        .HasForeignKey("team_id")
                        .HasConstraintName("FK_team_members_teams_team_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("team_id", "person_id");
                        j.HasIndex("team_id", "person_id").IsUnique();
                    });

            modelBuilder.Entity<Entry>()
                .HasIndex(e => new { e.CompetitionId, e.ParticipantId, e.DisciplineId })
                .IsUnique();

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Entry)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EntryId);

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EntryId)
                .IsUnique();

            modelBuilder.Entity<Judge>()
                .HasOne(j => j.Person)
                .WithOne()
                .HasForeignKey<Judge>(j => j.PersonId);

            modelBuilder.Entity<Competition>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Discipline>()
                .HasOne(d => d.Apparatus)
                .WithMany()
                .HasForeignKey(d => d.ApparatusId);

            modelBuilder.Entity<Appeal>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId);

            modelBuilder.Entity<Entry>()
                .HasOne(e => e.Competition)
                .WithMany()
                .HasForeignKey(e => e.CompetitionId);

            modelBuilder.Entity<Entry>()
                .HasOne(e => e.Discipline)
                .WithMany()
                .HasForeignKey(e => e.DisciplineId);

            modelBuilder.Entity<Entry>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Entry>()
                .Property(e => e.ApplicationStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Entry>()
                .Property(e => e.EntryStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Appeal>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Score>()
                .Property(s => s.Type)
                .HasConversion<string>();
        }
    }
}
