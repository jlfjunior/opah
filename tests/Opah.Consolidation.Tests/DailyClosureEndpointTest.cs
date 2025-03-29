using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Opah.Consolidation.Application;
using Opah.Consolidation.Domain;
using Opah.Consolidation.Infrastructure;

namespace Opah.Consolidation.Tests;

public class DailyClosureEndpointTest : IClassFixture<CustomProgramFactory>
{
    readonly CustomProgramFactory _server;
    public DailyClosureEndpointTest(CustomProgramFactory factory)
    {
        _server = factory;
    }
    
    [Fact]
    public async Task GetDailyClosure_WhenNotFound_ShouldReturnOk()
    {
        var client = _server.CreateClient();
        
        var response = await client.GetAsync("/daily-closures?referenceDate=2020-08-15");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDailyClosure_WhenValid_ShouldReturnOk()
    {
        var client = _server.CreateClient();
        var dbContext = _server.Services.GetRequiredService<ConsolidationDbContext>();

        var dailyClosure = new DailyClosure(new DateOnly(2020, 08, 15));
        var transaction1 = new Transaction
        {
            ReferenceDate = dailyClosure.ReferenceDate,
            Value = 200,
            Direction = Direction.Credit
        };
        
        var transaction2 = new Transaction
        {
            ReferenceDate = dailyClosure.ReferenceDate,
            Value = 100,
            Direction = Direction.Credit
        };
        
        dailyClosure.AddTransaction(transaction1);
        dailyClosure.AddTransaction(transaction2);
        
        dbContext.Set<DailyClosure>().Add(dailyClosure);
        await dbContext.SaveChangesAsync();
        
        var response = await client.GetAsync("/daily-closures?referenceDate=2020-08-15");
        var responseDate = await response.Content.ReadFromJsonAsync<DailyClosureResponse>();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        responseDate.Value.Should().Be(dailyClosure.Value);
    }
    
    [Fact]
    public async Task GetDailyClosure_WhenMultipleDaysExist_ShouldReturnCorrectDay()
    {
        var client = _server.CreateClient();
        var dbContext = _server.Services.GetRequiredService<ConsolidationDbContext>();
    
        var dailyClosure1 = new DailyClosure(new DateOnly(2020, 08, 15));
        dailyClosure1.AddTransaction(new Transaction { ReferenceDate = dailyClosure1.ReferenceDate, Value = 200, Direction = Direction.Credit });
    
        var dailyClosure2 = new DailyClosure(new DateOnly(2020, 08, 16));
        dailyClosure2.AddTransaction(new Transaction { ReferenceDate = dailyClosure2.ReferenceDate, Value = 300, Direction = Direction.Credit });
    
        dbContext.Set<DailyClosure>().AddRange(dailyClosure1, dailyClosure2);
        await dbContext.SaveChangesAsync();
    
        var response = await client.GetAsync("/daily-closures?referenceDate=2020-08-16");
        var responseDate = await response.Content.ReadFromJsonAsync<DailyClosureResponse>();
    
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseDate.Value.Should().Be(dailyClosure2.Value);
    }
}