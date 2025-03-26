using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Application;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;
using StackExchange.Redis;

namespace Opah.Consolidation.API;

public static class DailyClosureEndpoints
{
    public static void MapDailyClosureEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("daily-closure");
        
        group.MapGet("/", async (DateOnly referenceDate, IDailyClosureService service) =>
        {
            var dailyClosures = await service.ListAsync(referenceDate);

            return Results.Ok(dailyClosures);
        });
    }
}