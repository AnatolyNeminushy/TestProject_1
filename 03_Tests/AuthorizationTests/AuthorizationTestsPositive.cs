using System.Net;
using System.Text.Json;

namespace Authorization;
using Clients;
using BaseSpaceRequest;

public class AuthorizationTestsPositive
{
    
    private RequestsClients ClientRequest ;
    private RequestsAuthorization RequestsAuthorization;
   
   public AuthorizationTestsPositive()
    {
        ClientRequest = new RequestsClients();
        RequestsAuthorization = new RequestsAuthorization(ClientRequest);
    }

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
        // 
        // Accounts
        var responseAccounts = await RequestsAuthorization.UniversalGetRequestAfterAuthorization("api/accounts"); 
        Console.WriteLine($"Accounts-StatusCode:{responseAccounts.StatusCode}");
        Console.WriteLine($"Accounts-Content:{responseAccounts.Content}");
        // Console.WriteLine($"Accounts-AccessToken:{ClientRequest.serializationDataClients?.AccessToken}");
        Assert.Equal(HttpStatusCode.OK, responseAccounts.StatusCode);
       

        // Cards
        var responseCards = await RequestsAuthorization.UniversalGetRequestAfterAuthorization("api/cards"); 
        Console.WriteLine($"Cards-StatusCode:{responseCards.StatusCode}");
        Console.WriteLine($"Cards-Content:{responseCards.Content}");
        // Console.WriteLine($"Cards-AccessToken:{ClientRequest.serializationDataClients?.AccessToken}");
        Assert.Equal(HttpStatusCode.OK, responseCards.StatusCode);

        // CardsOrders
        var responseCardsOrders = await RequestsAuthorization.UniversalGetRequestAfterAuthorization("api/cards/orders"); 
        Console.WriteLine($"CardsOrders-StatusCode:{responseCardsOrders.StatusCode}");
        Console.WriteLine($"CardsOrders-Content:{responseCardsOrders.Content}");
        // Console.WriteLine($"CardsOrders-AccessToken:{ClientRequest.serializationDataClients?.AccessToken}");
        Assert.Equal(HttpStatusCode.OK, responseCardsOrders.StatusCode);


       
        

        
        


    }
}