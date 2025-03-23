using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opah.Consolidation.Domain;

namespace Opah.Consolidation.Infrastructure;

public class DailyClosureConfiguration : IEntityTypeConfiguration<DailyClosure>
{
    public void Configure(EntityTypeBuilder<DailyClosure> builder)
    {
        builder.ToTable("DailyClosures");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ReferenceDate)
            .IsUnique();

        builder.Property(x => x.ReferenceDate);

        builder.Property(x => x.Value).HasPrecision(18, 2);

        builder.Property(x => x.Status);

        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
    }
}