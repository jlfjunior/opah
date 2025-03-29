using System.Net.Http.Json;
using FluentAssertions;
using Opah.Consolidation.Application;

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
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}