namespace Opah.Consolidation.Domain;

public class Transaction
{
    public Guid Id { get; set; }
    public DateOnly ReferenceDate { get; set; }
    public decimal Value { get; set; }
    public Direction Direction { get; set; }
    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
    public Guid DailyClosureId { get; set; }
    public DailyClosure DailyClosure { get; set; }
}