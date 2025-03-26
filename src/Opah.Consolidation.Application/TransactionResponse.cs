namespace Opah.Consolidation.Application;

public record TransactionResponse(Guid Id, decimal Value, DateOnly ReferenceDate, string Direction);