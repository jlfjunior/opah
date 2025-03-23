using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackExchange.Redis;

namespace Opah.Transaction.API;

public enum Direction
{
    Debit = 10,
    Credit = 20
}

public class Transaction : Entity
{
    protected Transaction() { }
    
    public Transaction(DateOnly referenceDate, decimal value, Direction direction)
    {
        Id = Guid.NewGuid();
        ReferenceDate = referenceDate;
        Value = value;
        Direction = direction;
        
        Validate();
    }

    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public Direction Direction { get; private set; }
    
    private void Validate()
    {
        if (ReferenceDate == default) throw new Exception("Reference date is required");
        if (Value <= 0) throw new Exception("Value must be greater than 0");
    }
    
    public static Transaction Debit(DateOnly referenceDate, decimal value)
    {
        return new Transaction(referenceDate, value, Direction.Debit);
    }
    
    public static Transaction Credit(DateOnly referenceDate, decimal value)
    {
        return new Transaction(referenceDate, value, Direction.Credit);
    }
}

public record DebitTransactionRequest(DateOnly ReferenceDate, decimal Value);

public class DebitTransactionValidation : AbstractValidator<DebitTransactionRequest>
{
    public DebitTransactionValidation()
    {
        RuleFor(x => x.ReferenceDate)
            .NotEmpty();
        
        RuleFor(x => x.Value)
            .GreaterThan(0)
            .NotEmpty();
    }
}

public record CreditTransactionRequest(DateOnly ReferenceDate, decimal Value);

public class CreditTransactionValidation : AbstractValidator<CreditTransactionRequest>
{
    public CreditTransactionValidation()
    {
        RuleFor(x => x.ReferenceDate)
            .NotEmpty();
        
        RuleFor(x => x.Value)
            .GreaterThan(0)
            .NotEmpty();
    }
}

public record TransactionResponse(Guid Id, decimal Value, DateOnly ReferenceDate, string Direction);

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/transactions");

        endpoints.MapPost("/debit", async (DebitTransactionRequest request, TransactionDbContext context, TransactionService service) =>
        {
            var validation = new DebitTransactionValidation().Validate(request);
            
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var transaction = Transaction.Debit(request.ReferenceDate, request.Value);
            
            context.Add(transaction);
            await context.SaveChangesAsync();
            
            //TODO: I can use Mappers to turn the code simple 
            var response = new TransactionResponse(
                transaction.Id, 
                transaction.Value, 
                transaction.ReferenceDate,
                "Debit");

            await service.PublishAsync(response);
            
            return Results.Created("transactions/debit", response);
        });

        endpoints.MapPost("/credit", async (CreditTransactionRequest request, TransactionDbContext context, TransactionService service) =>
        {
            var validation = new CreditTransactionValidation().Validate(request);
            
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var transaction = Transaction.Credit(request.ReferenceDate, request.Value);
            
            context.Add(transaction);
            await context.SaveChangesAsync();
            
            var response = new TransactionResponse(
                transaction.Id, 
                transaction.Value, 
                transaction.ReferenceDate,
                "Credit");
            
            await service.PublishAsync(response);
            
            return Results.Created("transactions/credit", response);
        });

        endpoints.MapGet("/", async (TransactionDbContext context) =>
        {
            var transactions = await context.Set<Transaction>().ToListAsync();
            
            return Results.Ok(transactions);
        });
    }
}


public class TransactionService
{
    public async Task PublishAsync(TransactionResponse response)
    {
        var redisConnectionString = "localhost:6379";
        var connection = ConnectionMultiplexer.Connect(redisConnectionString);
        var database = connection.GetDatabase();
            
        var topic = "queuing.transactions.created";

        var entries = new NameValueEntry[]
        {
            new NameValueEntry("id", response.Id.ToString()),
            new NameValueEntry("direction", response.Direction),
            new NameValueEntry("value", response.Value.ToString()),
            new NameValueEntry("referenceDate", response.ReferenceDate.ToString()),
        };
            
        await database.StreamAddAsync(topic, entries);
    }
}