using System.Net;
using System.Text.Json;

namespace Clients;

using BaseSpaceRequest;

public class CreateClientTestsPositive
{
    private RequestsClients requestClients = new RequestsClients();
    private AuthenticationToken postRequestToken = new AuthenticationToken();

    [Fact]
    public async Task CreateNewClientTest()
    {
        // Arrange
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var (responseCreateClient, dataClients) =
            await requestClients.CreateRequestForEndpointClients();
        Assert.NotNull(responseCreateClient.Content);

        var login = dataClients.Login;
        var password = dataClients.Password;
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
        var responseGetClient = await requestClients.GetRequestForEndpointClients(
            "api/clients",
            accessToken
        );

        Assert.NotNull(responseGetClient.Content);
        var getDataAfterCreateClientDeserialize = JsonSerializer.Deserialize<DataClients>(
            responseGetClient.Content,
            options
        );
        Assert.NotNull(getDataAfterCreateClientDeserialize);
        // Проверил, что лежит внутри getDeserialize
        // Console.WriteLine(getDeserialize.Login);
        // Console.WriteLine(getDeserialize.Email);
        //
        // Asserts
        Console.WriteLine($"[CREATE] Status Code:{responseCreateClient.StatusCode}");
        Console.WriteLine($"[AUTH_TOKEN] Status Code:{autToken.StatusCode}");
        Console.WriteLine($"[GET_CLIENT] Status Code:{responseGetClient.StatusCode}");
        Console.WriteLine($"[CONTENT_GET_CLIENT] Content:{responseGetClient.Content}");

        Assert.Equal(HttpStatusCode.OK, responseCreateClient.StatusCode);
        Assert.Equal(HttpStatusCode.OK, autToken.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGetClient.StatusCode);

        Assert.Equal(dataClients.Login, getDataAfterCreateClientDeserialize.Login);
        Assert.Equal(dataClients.Email, getDataAfterCreateClientDeserialize.Email);
    }
}
