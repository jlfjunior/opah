namespace Opah.Consolidation.Domain;

public class Transaction : Entity
{
    public DateOnly ReferenceDate { get; set; }
    public decimal Value { get; set; }
    public Direction Direction { get; set; }
    public Guid DailyClosureId { get; set; }
    public DailyClosure DailyClosure { get; set; }
}