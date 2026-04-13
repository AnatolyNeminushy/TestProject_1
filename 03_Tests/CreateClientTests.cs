using BaseSpaceRequests;
using RestSharp;
using System.Net;
using Tools;

namespace Clients;

/// <summary>
/// Тесты на создание пользователя.
/// </summary>
public class CreateClientTests
{
    private RestClients restClients = new RestClients();
    private AuthenticationClient authenticationClient =
        new AuthenticationClient();

    /// <summary>
    /// Создание пользователя с валдиными данными.
    /// </summary>
    [Fact]
    public async Task CreateNewClient_WithNewData_ReturnsOk()
    {
        // Arrange
        var unique = DateTime.Now.Ticks;
        var random = new Random();
        var phoneNumber = "+7" + random.Next(100000000, 1000000000).ToString();
        var start = new DateTime(1970, 1, 1);
        var end = new DateTime(2005, 12, 31);
        var randomDate = start.AddDays(random.Next((end - start).Days));
        var formattedDate = randomDate.ToString("yyyy-MM-dd");
        var data = new DataClients
        {
            PhoneNumber = phoneNumber,
            Login = $"user{unique}",
            Email = $"user{unique}@email.ru",
            FirstName = $"name{unique}",
            LastName = $"lastname{unique}",
            MiddleName = $"middlenamfffffe{unique}",
            Sex = "Female",
            Address = $"address{unique}",
            Birthdate = formattedDate,
            Password = $"password{unique}",
        };

        var createClientResponse = await restClients.CreateClient(data);

        var authenticationResponse =
            await authenticationClient
                .RequestToObtainAuthenticationToken(data.Login, data.Password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var getClientRequest =
            restClients
                .CreateBaseRequest("api/clients", Method.Get, accessToken);

        // Act
        var getClientResponse =
            await restClients.Client.ExecuteAsync(getClientRequest);
        var getClientData =
            JsonDeserializer.DeserializeData<DataClients>(
                getClientResponse.Content);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, createClientResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, authenticationResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getClientResponse.StatusCode);

        Assert.Equal(data.Login, getClientData.Login);
        Assert.Equal(data.Email, getClientData.Email);
    }

    /// <summary>
    /// Создание уже существующего пользователя.
    /// </summary>
    [Fact]
    public async Task CreateNewClient_WithExistingData_ReturnsBadRequest()
    {
        // Arrange
        var registeredDataClient = new DataClients
        {
            Login = "DimaFire",
            Email = "dmitrytkachenko@yandex.ru",
            PhoneNumber = "+79788902369",
            FirstName = "Дмитрий",
            LastName = "Ткаченко",
            MiddleName = "Андреевич",
            Sex = "Male",
            Address = "ул. Ленина, 45",
            Birthdate = "1998-12-06",
            Password = "Dima5678",
        };

        // Act
        var responseNegativeCreateClient =
            await restClients.CreateClient(registeredDataClient);

        // Asserts
        Assert.Equal(
            HttpStatusCode.BadRequest,
            responseNegativeCreateClient.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1234")]
    [InlineData("dfdgdg")]
    [InlineData("@#$$#")]
    [InlineData("")]
    public async Task CreateNewClient_WithInvalidSex_ReturnsBadRequest(
        string sex)
    {
        // Arrange
        var titleError = "One or more validation errors occurred.";
        var registeredDataClient = new DataClients
        {
            Login = "DimaFire",
            Email = "dmitrytkachenko@yandex.ru",
            PhoneNumber = "+79788902369",
            FirstName = "Дмитрий",
            LastName = "Ткаченко",
            MiddleName = "Андреевич",
            Sex = sex,
            Address = "ул. Ленина, 45",
            Birthdate = "1998-12-06",
            Password = "Dima5678",
        };

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(registeredDataClient);
        var сreateClientData = JsonDeserializer
            .DeserializeData<ErrorCreateClient>(сreateClientResponse.Content);

        Console.WriteLine(сreateClientData.Title);
        // Asserts
        Assert.Equal(
            HttpStatusCode.BadRequest, сreateClientResponse.StatusCode);
        Assert.Equal(
            titleError, сreateClientData.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1234")]
    [InlineData("dfdgdg")]
    [InlineData("@#$$#")]
    [InlineData("")]
    public async Task CreateNewClient_WithInvalidEmail_ReturnsBadRequest(
        string email)
    {
        // Arrange
        var titleError = "One or more validation errors occurred.";
        var registeredDataClient = new DataClients
        {
            Login = "DimaFire",
            Email = email,
            PhoneNumber = "+79788902369",
            FirstName = "Дмитрий",
            LastName = "Ткаченко",
            MiddleName = "Андреевич",
            Sex = "Male",
            Address = "ул. Ленина, 45",
            Birthdate = "1998-12-06",
            Password = "Dima5678",
        };

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(registeredDataClient);
        var сreateClientData = JsonDeserializer
            .DeserializeData<ErrorCreateClient>(сreateClientResponse.Content);

        Console.WriteLine(сreateClientData.Title);
        // Asserts
        Assert.Equal(
            HttpStatusCode.BadRequest, сreateClientResponse.StatusCode);
        Assert.Equal(
            titleError, сreateClientData.Title);
    }

}
