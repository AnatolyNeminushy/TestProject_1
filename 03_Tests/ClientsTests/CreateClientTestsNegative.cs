using System.Net;
using System.Text.Json;

namespace Clients;

using BaseSpaceRequest;

public class CreateClientTestsNegative
{
    private RequestsClients requestsClients = new RequestsClients();

    [Fact]
    public async Task CreateNewClientTest()
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // Act
        var registeredDataClient = new DataClients
        {
            PhoneNumber = "+79788902369",
            Login = "DimaFire",
            Email = "dmitrytkachenko@yandex.ru",
            FirstName = "Дмитрий",
            LastName = "Ткаченко",
            MiddleName = "Андреевич",
            Sex = "Male",
            Address = "ул. Ленина, 45",
            Birthdate = "1998-12-06",
            Password = "Dima5678",
        };
        // Asserts
        //
        // Негативный сценарий
        // Create
        var (responseNegativeCreateClient, dataClients) =
            await requestsClients.CreateRequestForEndpointClients(registeredDataClient);
        // var responseNegativeCreateClient = await requestsClients.CreateRequestForEndpointClients(
        //     registeredDataClient
        // );
        Console.WriteLine(
            $"[NEGATIVE_CREATE] StatusCode:{responseNegativeCreateClient.StatusCode}"
        );

        Assert.Equal(HttpStatusCode.BadRequest, responseNegativeCreateClient.StatusCode);
    }
}
