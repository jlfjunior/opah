using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;
using StackExchange.Redis;

namespace Opah.Consolidation.API;

public record TransactionResponse(Guid Id, decimal Value, DateOnly ReferenceDate, string Direction);

public record DailyClosureResponse(DateOnly ReferenceDate, decimal Value, string Status);

public static class DailyClosureEndpoints
{
    public static void MapDailyClosureEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("daily-closure");
        
        group.MapGet("/", async (DateOnly referenceDate, ConsolidationDbContext context) =>
        
        {
            var dailyClosures = await context.Set<DailyClosure>()
                .Where(c => c.ReferenceDate == referenceDate)
                .AsNoTracking()
                .Select(x => new DailyClosureResponse(x.ReferenceDate, x.Value, x.Status == DailyClosureStatus.Closed ? "Closed" : "Open")
                ).ToListAsync();
            
            return Results.Ok(dailyClosures);
        });
    }
}