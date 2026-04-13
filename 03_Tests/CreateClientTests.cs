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
    private AuthenticationClient authenticationClient = new AuthenticationClient();
    // TODO приватный метод создать


    private DataClients getDataClients(string nameParametr = null, string value = null)
    {
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
        if (string.IsNullOrEmpty(nameParametr) || string.IsNullOrEmpty(value))
        {
            return data;
        }
        _ = nameParametr switch
        {
            "login" => data.Login = value,
            "sex" => data.Sex = value,
            "email" => data.Email = value,
            "password" => data.Password = value,
            "birthdate" => data.Birthdate = value,
            _ => throw new ArgumentException("Неверный параметр")
        };

        return data;
    }

    /// <summary>
    /// Создание пользователя с валидными данными.
    /// </summary>
    [Fact]
    public async Task CreateNewClient_WithNewData_ReturnsOk()
    {
        // Arrange
        var data = getDataClients();

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
        var data = getDataClients("sex", sex);
        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("dfdgdg")]
    [InlineData("@#$$#")]
    public async Task CreateNewClient_WithInvalidEmail_ReturnsBadRequest(
        string email)
    {
        // Arrange
        var data = getDataClients("email", email);

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateNewClient_WithInvalidLogin_ReturnsBadRequest(
       string login)
    {
        // Arrange
        var data = getDataClients("login", login);

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456dfsd")]
    [InlineData("fkkoe454ldlsl343lssl5")]
    public async Task CreateNewClient_WithInvalidPassword_ReturnsBadRequest(
       string password)
    {
        // Arrange
        var data = getDataClients("password", password);

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("2020-12-03")]
    public async Task CreateNewClient_WithInvalidBirthDate_ReturnsBadRequest(
   string birthdate)
    {
        // Arrange
        var data = getDataClients("birthdate", birthdate);

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("+67050-5")]
    [InlineData("+798756778980")]
    public async Task CreateNewClient_WithInvalidPhoneNumber_ReturnsBadRequest(
   string phonenumber)
    {
        // Arrange
        var data = getDataClients("phonenumber", phonenumber);

        // Act
        var сreateClientResponse =
            await restClients.CreateClient(data);

        // Asserts
        Assert.Equal(
            HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

}
