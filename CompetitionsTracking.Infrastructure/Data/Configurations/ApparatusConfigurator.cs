using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class ApparatusConfigurator : IEntityTypeConfiguration<Apparatus>
    {
        public void Configure(EntityTypeBuilder<Apparatus> builder)
        {
            builder.HasMany(a => a.Disciplines)
                   .WithOne(d => d.Apparatus)
                   .HasForeignKey(d => d.ApparatusId);
        }
    }
}
