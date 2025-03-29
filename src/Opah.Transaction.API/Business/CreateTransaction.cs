using AutoMapper;
using MediatR;
using Opah.Redis.Client;
using Opah.Transaction.API.Infrastructure;

namespace Opah.Transaction.API.Business;

public class CreateTransactionCommand : IRequest<TransactionResponse>
{
    public DateOnly ReferenceDate { get; set; }
    public decimal Value { get; set; }
    public string Type { get; set; }
}

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionResponse>
{
    readonly ILogger<CreateTransactionCommandHandler> _logger;
    readonly TransactionDbContext _context;
    readonly IStreamPublisher _publisher;
    readonly IMapper _mapper;

    public CreateTransactionCommandHandler(ILogger<CreateTransactionCommandHandler> logger, TransactionDbContext context, IStreamPublisher publisher, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _publisher = publisher;
        _mapper = mapper;
    }
    
    public async Task<TransactionResponse> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        Transaction transaction;

        switch (request.Type.ToLower())
        {
            case "debit": transaction = Transaction.Debit(request.ReferenceDate, request.Value); break;
            case "credit": transaction = Transaction.Credit(request.ReferenceDate, request.Value); break;
            default: throw new Exception();
        }
        
        _context.Add(transaction);
        await _context.SaveChangesAsync();
            
        var response = _mapper.Map<Transaction, TransactionResponse>(transaction);

        await _publisher.ProducerAsync<TransactionResponse>("transactions.created", response);
        return response;
    }
}
