using System.Net;
using System.Text.Json;

namespace Authorization;
using Clients;
using BaseSpaceRequest;

public class AuthorizationTestsNegative
{
    
    private RequestsClients ClientRequest ;
    private RequestsAuthorization RequestsAuthorization;
   
   public AuthorizationTestsNegative()
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

        var accessToken = "repfppdprgrp";

        // Asserts
        // 
        // Негативный сценарий
        // 
        // Accounts
        

        var responseAccounts = await RequestsAuthorization.UniversalNegativeGetRequestAfterAuthorization("api/accounts",accessToken); 
        Console.WriteLine($"NegativeTestAccounts-StatusCode:{responseAccounts.StatusCode}");
        Console.WriteLine($"NegativeTestAccounts-Content:{responseAccounts.Content}");
        // Console.WriteLine($"NegativeTestAccounts-AccessToken:{accessToken}");
        
        Assert.Equal(HttpStatusCode.Unauthorized, responseAccounts.StatusCode);

        // Cards
        var responseCards = await RequestsAuthorization.UniversalNegativeGetRequestAfterAuthorization("api/cards",accessToken); 
        Console.WriteLine($"NegativeTestCards-StatusCode:{responseCards.StatusCode}");
        Console.WriteLine($"NegativeTestCards-Content:{responseCards.Content}");
        // Console.WriteLine($"NegativeTestCards-AccessToken:{accessToken}");
        Assert.Equal(HttpStatusCode.Unauthorized, responseCards.StatusCode);

        // CardsOrders
        var responseCardsOrders = await RequestsAuthorization.UniversalNegativeGetRequestAfterAuthorization("api/cards/orders",accessToken); 
        Console.WriteLine($"NegativeTestCardsOrders-StatusCode:{responseCardsOrders.StatusCode}");
        Console.WriteLine($"NegativeTestCardsOrders-Content:{responseCardsOrders.Content}");
        // Console.WriteLine($"NegativeTestCardsOrders-AccessToken:{accessToken}");
        Assert.Equal(HttpStatusCode.Unauthorized, responseCardsOrders.StatusCode);


       
        

        
        


    }
}