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
        
        Transactions.Add(transaction);
        Value += transaction.Value;
        transaction.DailyClosureId = Id;
    }
}