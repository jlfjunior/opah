namespace Opah.Consolidation.Domain;

public class DailyClosure : Entity
{
    protected DailyClosure() { }

    public DailyClosure(DateOnly referenceDate)
    {
        ReferenceDate = referenceDate;
        Status = DailyClosureStatus.Open;
    }
    
    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public DailyClosureStatus Status { get; private set; }
    public ICollection<Transaction> Transactions { get; set; }

    public void AddTransaction(Transaction transaction)
    {
        if (Transactions == null)
            Transactions = new List<Transaction>();
        
        transaction.DailyClosureId = Id;
        
        Transactions.Add(transaction);
        
        if (transaction.Direction == Direction.Credit)
            Value += transaction.Value;
        else
            Value -= transaction.Value;
    }
}