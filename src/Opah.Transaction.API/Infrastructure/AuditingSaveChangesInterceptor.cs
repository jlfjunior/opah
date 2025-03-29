using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Opah.Transaction.API.Business;

namespace Opah.Transaction.API.Infrastructure;

public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken token = new CancellationToken())
    {
        var entries = eventData.Context.ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Where(e => e.Entity is Entity);

        foreach (var entry in entries)
        {
             if (entry.State == EntityState.Added)
                 entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
             else if (entry.State == EntityState.Modified)
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }
        
        return base.SavingChangesAsync(eventData, result, token);
    }
}