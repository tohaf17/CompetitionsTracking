using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class EntryConfigurator:IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> modelBuilder)
        {
            modelBuilder
                .HasOne(e => e.Competition)
                .WithMany(c => c.Entries)
                .HasForeignKey(e => e.CompetitionId);
            modelBuilder
                .HasOne(e => e.Participant)
                .WithMany(p => p.Entries)
                .HasForeignKey(e => e.ParticipantId);
            modelBuilder
                .HasOne(e => e.Discipline)
                .WithMany(d => d.Entries)
                .HasForeignKey(e => e.DisciplineId);
            modelBuilder
                .HasOne(e => e.Category)
                .WithMany(c => c.Entries)
                .HasForeignKey(e => e.CategoryId);
            modelBuilder
                .Property(e => e.ApplicationStatus)
                .HasConversion<string>();
            modelBuilder
                .Property(e => e.EntryStatus)
                .HasConversion<string>();
        }
    }
}
