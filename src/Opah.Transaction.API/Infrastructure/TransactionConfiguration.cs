using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Opah.Transaction.API.Infrastructure;

public class TransactionConfiguration : IEntityTypeConfiguration<Business.Transaction>
{
    public void Configure(EntityTypeBuilder<Business.Transaction> builder)
    {
        builder.ToTable(name: "Transactions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReferenceDate);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.Direction).HasConversion<int>();
        builder.Property(x => x.CreatedAt);
        
        builder.Ignore(x => x.UpdatedAt);
    }
}