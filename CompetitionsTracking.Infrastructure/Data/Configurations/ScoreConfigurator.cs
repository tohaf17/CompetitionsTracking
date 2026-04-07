using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class ScoreConfigurator : IEntityTypeConfiguration<Score>
    {
        public void Configure(EntityTypeBuilder<Score> builder)
        {
            builder.HasOne(s => s.Entry)
                   .WithMany(e => e.Scores)
                   .HasForeignKey(s => s.EntryId);

            builder.HasOne(s => s.Judge)
                   .WithMany(j => j.Scores)
                   .HasForeignKey(s => s.JudgeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.Type)
                   .HasConversion<string>();
        }
    }
}
