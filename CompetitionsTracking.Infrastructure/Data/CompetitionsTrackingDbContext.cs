using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CompetitionsTracking.Domain.Entities;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompetitionsTrackingDbContext).Assembly);
        }
    }
}