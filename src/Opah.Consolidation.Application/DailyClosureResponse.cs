namespace Opah.Consolidation.Application;

public record DailyClosureResponse(DateOnly ReferenceDate, decimal Value, string Status);
