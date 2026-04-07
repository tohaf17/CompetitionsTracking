using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class AppealConfigurator : IEntityTypeConfiguration<Appeal>
    {
        public void Configure(EntityTypeBuilder<Appeal> builder)
        {
            builder.HasOne(a => a.Result)
                   .WithMany(r => r.Appeals)
                   .HasForeignKey(a => a.ResultId);

            builder.Property(a => a.Status)
                   .HasConversion<string>();
        }
    }
}
