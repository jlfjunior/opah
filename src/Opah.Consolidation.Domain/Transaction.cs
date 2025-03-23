namespace Opah.Consolidation.Domain;

public class Transaction
{
    public Guid Id { get; private set; }
    public DateOnly ReferenceDate { get; private set; }
    public decimal Value { get; private set; }
    public Direction Direction { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid DailyClosureId { get; set; }
    public DailyClosure DailyClosure { get; set; }
}