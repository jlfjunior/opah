using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;
using StackExchange.Redis;
using Transaction = Opah.Consolidation.Domain.Transaction;

namespace Opah.Consolidation.Worker;

public class TransactionCreatedWorker : BackgroundService
{
    private readonly ILogger<TransactionCreatedWorker> _logger;
    private readonly IServiceProvider _provider;
    public TransactionCreatedWorker(ILogger<TransactionCreatedWorker> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<TransactionService>();
        var lastId = "0";
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            lastId = await service.Consumer(lastId);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

public record TransactionResponse(Guid Id, decimal Value, DateOnly ReferenceDate, string Direction);

public class TransactionService
{
    readonly ILogger<TransactionService> _logger;
    readonly ConsolidationDbContext _context;

    public TransactionService(ILogger<TransactionService> logger, ConsolidationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task AddTransaction(TransactionResponse response)
    {
        var dailyClosure = await _context.Set<DailyClosure>()
            .Where(x => x.ReferenceDate == response.ReferenceDate)
            .SingleOrDefaultAsync();

        if (dailyClosure == null)
        {
            dailyClosure = new DailyClosure(response.ReferenceDate);

            _context.Set<DailyClosure>().Add(dailyClosure);
            await _context.SaveChangesAsync();
        }

        var transaction = new Transaction()
        {
            ReferenceDate = response.ReferenceDate,
            Value = response.Value,
            Direction = response.Direction == "Credit" ? Direction.Credit : Direction.Debit
        };
        
        _context.Set<Transaction>().Add(transaction);
        dailyClosure.AddTransaction(transaction);
        
        
        await _context.SaveChangesAsync();
    }

    public async Task<string> Consumer(string lastId = "0")
    {
        var redisConnectionString = "localhost:6379";
        var connection = ConnectionMultiplexer.Connect(redisConnectionString);
        var topic = "queuing.transactions.created";
        var database = connection.GetDatabase();

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

            await AddTransaction(transaction);

            return lastId;
        }

        return lastId;
    }
}