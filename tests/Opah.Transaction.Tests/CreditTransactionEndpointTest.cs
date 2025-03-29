using System.Net.Http.Json;
using FluentAssertions;
using Opah.Transaction.API;
using Opah.Transaction.API.Business;

namespace Opah.Transaction.Tests;

public class CreditTransactionEndpointTest : IClassFixture<CustomProgramFactory>
{
    readonly CustomProgramFactory _server;
    public CreditTransactionEndpointTest(CustomProgramFactory factory)
    {
        _server = factory;
    }
    
    [Fact]
    public async Task CreateCreditTransactionAsync_ShouldReturnOk()
    {
        var client = _server.CreateClient();
        var request = new CreditTransactionRequest(new DateOnly(2025, 01, 01), 100.00m);
        
        var response = await client.PostAsJsonAsync("/transactions/credit", request);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        
        var responseData = await response.Content.ReadFromJsonAsync<TransactionResponse>();
        responseData.Should().NotBeNull();
        responseData.Value.Should().Be(100.00m);
        responseData.Direction.Should().Be("Credit");
    }
}