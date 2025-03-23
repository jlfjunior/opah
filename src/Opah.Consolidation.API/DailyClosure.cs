using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Domain;
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

public class TransactionCreatedHostedService : IHostedService
{
    readonly ILogger<TransactionCreatedHostedService> _logger;
    readonly TransactionService _transactionService;

    public TransactionCreatedHostedService(ILogger<TransactionCreatedHostedService> logger, IServiceProvider services)
    {
        _logger = logger;
        _transactionService = services.GetService<TransactionService>();
    }
    
    public async Task StartAsync(CancellationToken token)
    {
        _logger.LogInformation($"Starting {nameof(TransactionCreatedHostedService)}");

        while (!token.IsCancellationRequested)
            await _transactionService.Consumer();
    }

    public Task StopAsync(CancellationToken token)
    {
        _logger.LogInformation($"Stopping {nameof(TransactionCreatedHostedService)}");
        
        return Task.CompletedTask;
    }
}

public class TransactionService
{
    readonly ILogger<TransactionService> _logger;

    public TransactionService(ILogger<TransactionService> logger)
    {
        _logger = logger;
    }

    public async Task Consumer()
    {
        var redisConnectionString = "localhost:6379";
        var connection = ConnectionMultiplexer.Connect(redisConnectionString);
        var topic = "queuing.transactions.created";
        var subscriber = connection.GetSubscriber();
        var database = connection.GetDatabase();
        var lastId = "0";

        var streams = await database.StreamReadAsync(topic, lastId, 1);

        if (streams.Length > 0)
        {
            var entry = streams[0];
            lastId = entry.Id;

            var transaction = new TransactionResponse(
                Guid.Parse(entry["id"]),
                decimal.Parse(entry["value"]),
                DateOnly.Parse(entry["referenceDate"]),
                entry["direction"]
            );
            
            _logger.LogInformation($"Received transaction {transaction.Id}: {transaction.Value}");
        }
        
        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
    }
}