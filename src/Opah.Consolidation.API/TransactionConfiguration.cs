using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opah.Consolidation.API;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable(name: "Transactions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReferenceDate);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.Direction).HasConversion<int>();
        builder.Property(x => x.CreatedAt);

        builder.HasOne(x => x.DailyClosure)
            .WithMany(x => x.Transactions)
            .HasForeignKey(f => f.DailyClosureId);
    }
}