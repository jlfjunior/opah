using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;

namespace Opah.Consolidation.Application;

public interface IDailyClosureService
{
    Task AddTransaction(TransactionResponse response);
    Task<List<DailyClosureResponse>> ListAsync(DateOnly referenceDate);
}

public class DailyClosureService : IDailyClosureService
{
    readonly ILogger<DailyClosureService> _logger;
    readonly ConsolidationDbContext _context;
    readonly IMapper _mapper;
    
    public DailyClosureService(ILogger<DailyClosureService> logger, ConsolidationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }
    
    public async Task AddTransaction(TransactionResponse response)
    {
        var dailyClosure = await _context.Set<DailyClosure>()
            .Where(x => x.ReferenceDate == response.ReferenceDate)
            .SingleOrDefaultAsync();

        if (dailyClosure == null)
        {
            dailyClosure = new DailyClosure(response.ReferenceDate);

            _context.Set<DailyClosure>().Add(dailyClosure);
            await _context.SaveChangesAsync();
        }

        var transaction = _mapper.Map<Transaction>(response);
        
        dailyClosure.AddTransaction(transaction);
        
        _context.Set<Transaction>().Add(transaction);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Transaction {transaction.ReferenceDate} was successfully added.");
    }
    
    public async Task<List<DailyClosureResponse>> ListAsync(DateOnly referenceDate)
    {
        var dailyClosures = await _context.Set<DailyClosure>()
            .Where(c => c.ReferenceDate == referenceDate)
            .AsNoTracking()
            .ToListAsync();
        
        return _mapper.Map<List<DailyClosureResponse>>(dailyClosures);
    }
}