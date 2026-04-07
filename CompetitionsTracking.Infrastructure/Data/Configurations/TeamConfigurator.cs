using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class TeamConfigurator : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasOne(t => t.Coach)
                   .WithMany(p => p.TeamsCoached)
                   .HasForeignKey(t => t.CoachId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Members)
                   .WithMany(p => p.TeamsAsMember)
                   .UsingEntity<Dictionary<string, object>>(
                       "team_members",
                       j => j.HasOne<Person>().WithMany().HasForeignKey("person_id").OnDelete(DeleteBehavior.Restrict),
                       j => j.HasOne<Team>().WithMany().HasForeignKey("team_id").OnDelete(DeleteBehavior.Restrict),
                       j => j.HasKey("team_id", "person_id")
                   );
        }
    }
}
