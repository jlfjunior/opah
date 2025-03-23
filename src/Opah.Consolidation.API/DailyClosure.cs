using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackExchange.Redis;

namespace Opah.Consolidation.API;

public enum DailyClosureStatus
{
    Open = 10,
    Closed = 20,
}

public enum Direction
{
    Debit = 10,
    Credit = 20
}

public class DailyClosure
{
    protected DailyClosure() { }
    
    public Guid Id { get; private set; }
    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public DailyClosureStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<Transaction> Transactions { get; set; }
}

public class Transaction
{
    public Guid Id { get; private set; }
    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public Direction Direction { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid DailyClosureId { get; set; }
    public DailyClosure DailyClosure { get; set; }
}

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