using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class DisciplineConfigurator : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder.HasOne(d => d.Apparatus)
                   .WithMany(a => a.Disciplines)
                   .HasForeignKey(d => d.ApparatusId);
        }
    }
}
