using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Opah.Transaction.API.Business;
using Opah.Transaction.API.Infrastructure;

namespace Opah.Transaction.API;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var endpoints = routes.MapGroup("/transactions");

        endpoints.MapPost("/debit", async (DebitTransactionRequest request, IMediator mediator, IMapper mapper) =>
        {
            var validation = new DebitTransactionValidation().Validate(request);
            
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var command = mapper.Map<CreateTransactionCommand>(request);
            var transactionResponse = await mediator.Send(command);
            
            return Results.Created("transactions/debit", transactionResponse);
        });

        endpoints.MapPost("/credit", async (CreditTransactionRequest request, IMediator mediator, IMapper mapper) =>
        {
            var validation = new CreditTransactionValidation().Validate(request);
            
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var command = mapper.Map<CreateTransactionCommand>(request);
            var transactionResponse = await mediator.Send(command);

            return Results.Created("transactions/credit", transactionResponse);
        });

        endpoints.MapGet("/", async (TransactionDbContext context) =>
        {
            var transactions = await context.Set<Business.Transaction>().ToListAsync();
            
            return Results.Ok(transactions);
        });
    }
}