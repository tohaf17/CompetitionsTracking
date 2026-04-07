using CompetitionsTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompetitionsTracking.Infrastructure.Data.Configurations
{
    public class PersonConfigurator : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasOne(p => p.Mentor)
                   .WithMany(p => p.Mentees)
                   .HasForeignKey(p => p.MentorId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Gender)
                   .HasConversion<string>();
        }
    }
}
