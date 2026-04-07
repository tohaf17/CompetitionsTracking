using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class ParticipantConfigurator : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.UseTptMappingStrategy()
                   .ToTable("participants");
        }
    }
}
