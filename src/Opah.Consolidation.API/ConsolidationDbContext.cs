using Microsoft.EntityFrameworkCore;

namespace Opah.Consolidation.API;

public class ConsolidationDbContext : DbContext
{
    public ConsolidationDbContext(DbContextOptions<ConsolidationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsolidationDbContext).Assembly);
    }
}