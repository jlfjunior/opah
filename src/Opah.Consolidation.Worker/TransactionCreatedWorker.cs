using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Opah.Consolidation.Application;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;
using Opah.Redis.Client;
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
        var service = scope.ServiceProvider.GetRequiredService<IDailyClosureService>();
        var publisher = scope.ServiceProvider.GetRequiredService<IStreamPublisher>();
         
        var lastId = "0";
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var topic = "queuing.transactions.created";

            var (key, transaction) = await publisher.ConsumerAsync<TransactionResponse>(topic, lastId);
            
            _logger.LogInformation($"Received transaction {transaction.Id}: {transaction.Value}");

            await service.AddTransaction(transaction);
            lastId = key;

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}