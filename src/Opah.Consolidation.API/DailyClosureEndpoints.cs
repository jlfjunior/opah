using Microsoft.AspNetCore.Mvc;
using Opah.Consolidation.Application;

namespace Opah.Consolidation.API;

public static class DailyClosureEndpoints
{
    public static void MapDailyClosureEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("daily-closures");
        
        group.MapGet("/", async (DateOnly referenceDate, [FromServices]IDailyClosureService service) =>
        {
            var dailyClosures = await service.GetAsync(referenceDate); 
            
            if (dailyClosures == null)
                return Results.NotFound();
            
            return Results.Ok(dailyClosures);
        });
    }
}