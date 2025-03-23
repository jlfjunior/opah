namespace Opah.Consolidation.Domain;

public class DailyClosure
{
    protected DailyClosure() { }

    public DailyClosure(DateOnly referenceDate)
    {
        ReferenceDate = referenceDate;
        Status = DailyClosureStatus.Open;
    }
    
    public Guid Id { get; private set; }
    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public DailyClosureStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<Transaction> Transactions { get; set; }

    public void AddTransaction(Transaction transaction)
    {
        if (Transactions == null)
            Transactions = new List<Transaction>();
        
        Transactions.Add(transaction);
        Value += transaction.Value;
        transaction.DailyClosureId = Id;
    }
}