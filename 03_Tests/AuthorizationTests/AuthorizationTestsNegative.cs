using System.Net;
using System.Text.Json;

namespace Authorization;

using Clients;

public class AuthorizationTestsNegative
{
    private RequestsClients requestsClients = new RequestsClients();

    [Fact]
    public async Task AuthorizationClientTest()
    {
        // Arrange
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var accessToken = "repfppdprgrp";

        // Act
        var responseNegativeGetAccounts = await requestsClients.GetRequestForEndpointClients(
            "api/accounts",
            accessToken
        );

        var responseNegativeGetCards = await requestsClients.GetRequestForEndpointClients(
            "api/cards",
            accessToken
        );

        var responseNegativeGetCardsOrders = await requestsClients.GetRequestForEndpointClients(
            "api/cards/orders",
            accessToken
        );

        // Asserts
        Console.WriteLine(
            $"[GET_CLIENT_ACCOUNTS] Status Code:{responseNegativeGetAccounts.StatusCode}"
        );
        // Console.WriteLine($"[CONTENT_GET_ACCOUNTS] Content:{responseNegativeGetAccounts.Content}");
        Console.WriteLine(
            $"[NEGATIVE_GET_CLIENT_CARDS] Status Code:{responseNegativeGetCards.StatusCode}"
        );
        // Console.WriteLine(
        //     $"[NEGATIVE_CONTENT_GET_CARDS] Content:{responseNegativeGetCards.Content}"
        // );
        Console.WriteLine(
            $"[NEGATIVE_GET_CLIENT_CARDS_ORDERS] Status Code:{responseNegativeGetCardsOrders.StatusCode}"
        );
        // Console.WriteLine(
        //     $"[NEGATIVE_CONTENT_GET_CARDS_ORDERS] Content:{responseNegativeGetCardsOrders.Content}"
        // );

        Assert.Equal(HttpStatusCode.Unauthorized, responseNegativeGetAccounts.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseNegativeGetCards.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, responseNegativeGetCardsOrders.StatusCode);
    }
}
