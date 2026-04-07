using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class CompetitionConfigurator:IEntityTypeConfiguration<Competition>
    {
        public void Configure(EntityTypeBuilder<Competition> modelBuilder)
        {
            modelBuilder
                .HasMany(c => c.Entries)
                .WithOne(e => e.Competition)
                .HasForeignKey(e => e.CompetitionId);

            modelBuilder
                .Property(c => c.Status)
                .HasConversion<string>();
        }
    }
}
