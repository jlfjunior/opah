using FluentValidation;

namespace Opah.Transaction.API.Business;

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

public class TransactionCreatedEvent
{
    public Guid Id { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public string Direction { get; set; }
    public decimal Value { get; set; }
}