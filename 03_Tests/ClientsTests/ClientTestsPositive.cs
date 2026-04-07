using System.Net;
using System.Text.Json;

namespace Clients;
using BaseSpaceRequest;

public class ClientTestsPositive
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
        // Позитивный сценарий
        // Create
        var responseCreate = await Request.RequestForEndpointCreateClients(); 
        Console.WriteLine($"Create-StatusCode:{responseCreate.StatusCode}");
        
        Assert.Equal(HttpStatusCode.OK, responseCreate.StatusCode);
        Assert.NotNull(Request.serializationDataClients);
        Console.WriteLine($"Serialization-Login:{Request.serializationDataClients.MiddleName}");
       

        // Authentification
        var responseToken = await Request.PostAuthentificationToken();
        Console.WriteLine($"Authentification Content:{responseToken.Content}");
        Assert.NotNull(responseToken.Content);
        Assert.NotEmpty(responseToken.Content);

    
        Assert.NotNull(Request.serializationDataClients.AccessToken);
        // Console.WriteLine($"Access Token:{Request.serializationDataClients.AccessToken}");



        // Get
        var responseGet = await Request.RequestForEndpointGetClients();
        Console.WriteLine($"Http-StatusCode Get-request:{responseGet.StatusCode}");
        Console.WriteLine($"Content-get:{responseGet.Content}");
        Console.WriteLine($"Content-getSer:{Request.serializationDataClients.Login},{Request.serializationDataClients.MiddleName}");
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
        Assert.NotNull(responseGet.Content);
        Assert.NotEmpty(responseGet.Content);

        var clientDataAfterGetRequest = JsonSerializer.Deserialize<DataClients>(responseGet.Content, options);
        // Assert.Equal(Request.serializationDataClients, clientDataAfterGetRequest);
        Assert.NotNull(clientDataAfterGetRequest);
        Console.WriteLine($"Client-Get:{clientDataAfterGetRequest.Login}, {clientDataAfterGetRequest.Email},{clientDataAfterGetRequest.Password},{Request.serializationDataClients.Password}");
        Console.WriteLine($"Client-Serialize:{Request.serializationDataClients.Login}, {Request.serializationDataClients.Email}");
        Assert.Equal(Request.serializationDataClients.Login, clientDataAfterGetRequest.Login);
        Assert.Equal(Request.serializationDataClients.Email, clientDataAfterGetRequest.Email);



       
        

        
        


    }
}