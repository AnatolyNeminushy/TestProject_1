using System.Net;
using System.Text.Json;

namespace Clients;
using BaseSpaceRequest;

public class ClientTestsNegative
{
    
    private RequestsClients Request = new RequestsClients();
   

    [Fact]
    public async Task CreateNewClientTest()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        

        // Asserts
        // 
        // Негативный сценарий
        // Create
        var responseNegativeCreate = await Request.RequestForEndpointNegativeCreateClients(); 
        Console.WriteLine($"CreateNegative-StatusCode:{responseNegativeCreate.StatusCode}");
       
        Assert.Equal(HttpStatusCode.BadRequest, responseNegativeCreate.StatusCode);

    }
}