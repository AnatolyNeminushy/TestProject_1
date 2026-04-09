using System.Net;
using System.Text.Json;

namespace Authorization;

using BaseSpaceRequest;
using Clients;

public class AuthorizationTestsPositive
{
    private RequestsClients requestsClients = new RequestsClients();
    private AuthenticationToken postRequestToken = new AuthenticationToken();

    [Fact]
    public async Task AuthorizationClientTest()
    {
        // Arrange
        // Тестовые данные пользователя
        //
        // {
        //  "phoneNumber": "+79788902369",
        //  "email": "dmitrytkachenko@yandex.ru",
        //  "login": "DimaFire",
        //  "address": "ул. Ленина, 45",
        //  "birthdate": "1998-12-06",
        //  "firstName": "Дмитрий",
        //  "lastName": "Ткаченко",
        //  "middleName": "Андреевич",
        //  "password": "Dima5678",
        //  "sex": "Male"
        // }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var login = "DimaFire";
        var password = "Dima5678";
        Assert.NotNull(login);
        Assert.NotNull(password);

        var autToken = await postRequestToken.RequestToObtainAuthenticationToken(login, password);
        Assert.NotNull(autToken.Content);
        var dataAuthTokenDeserialize = JsonSerializer.Deserialize<DataClients>(
            autToken.Content,
            options
        );
        Assert.NotNull(dataAuthTokenDeserialize);
        var accessToken = dataAuthTokenDeserialize.AccessToken;

        // Act
        Assert.NotNull(accessToken);
        var responseGetAccounts = await requestsClients.GetRequestForEndpointClients(
            "api/accounts",
            accessToken
        );
        var responseGetCards = await requestsClients.GetRequestForEndpointClients(
            "api/cards",
            accessToken
        );
        var responseGetCardsOrders = await requestsClients.GetRequestForEndpointClients(
            "api/cards/orders",
            accessToken
        );

        // Asserts
        Console.WriteLine($"[GET_CLIENT_ACCOUNTS] Status Code:{responseGetAccounts.StatusCode}");
        // Console.WriteLine($"[CONTENT_GET_ACCOUNTS] Content:{responseGetAccounts.Content}");
        Console.WriteLine($"[GET_CLIENT_CARDS] Status Code:{responseGetCards.StatusCode}");
        // Console.WriteLine($"[CONTENT_GET_CARDS] Content:{responseGetCards.Content}");
        Console.WriteLine(
            $"[GET_CLIENT_CARDS_ORDERS] Status Code:{responseGetCardsOrders.StatusCode}"
        );
        // Console.WriteLine($"[CONTENT_GET_CARDS_ORDERS] Content:{responseGetCardsOrders.Content}");

        Assert.Equal(HttpStatusCode.OK, responseGetAccounts.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetCards.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetCardsOrders.StatusCode);
    }
}
