using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentQueries.Domain;

namespace StudentQueries.Data.Configurations;

public class StudentsConfiguration : IEntityTypeConfiguration<Domain.Student>
{
    public void Configure(EntityTypeBuilder<Domain.Student> builder)
    {
        builder.Property(x => x.Id)
            .IsRequired()
            .HasConversion(
                x => x.ToString(),
                x => Guid.Parse(x)
            );

        builder.HasIndex(s => new { s.Id, s.Sequence })
            .IsUnique();
    }
}