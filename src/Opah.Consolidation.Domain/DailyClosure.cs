namespace Opah.Consolidation.Domain;

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