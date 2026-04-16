using RestSharp;
using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на создание пользователя.
/// </summary>
public class CreateClientTests : BaseTest
{
    /// <summary>
    /// Создание данных пользователя.
    /// </summary>
    /// <returns>Объект данных пользователя.</returns>
    private DataClients getDataClients()
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

        var createClientResponse = await ClientsRestClient.CreateClient(data);

        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(data.Login, data.Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var getClientResponse = await ClientsRestClient.GetClient(accessToken);
        var getClientData = JsonDeserializer.DeserializeData<DataClients>(getClientResponse.Content);

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
        var responseNegativeCreateClient = await ClientsRestClient.CreateClient(registeredDataClient);

        // Asserts
        Assert.Equal(HttpStatusCode.BadRequest, responseNegativeCreateClient.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным sex.
    /// </summary>
    /// <param name="sex">Пол пользователя.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("1234")]
    [InlineData("dfdgdg")]
    [InlineData("@#$$#")]
    [InlineData("")]
    public async Task CreateNewClient_WithInvalidSex_ReturnsBadRequest(string sex)
    {
        // Arrange
        var data = getDataClients();
        data.Sex = sex;
        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным email.
    /// </summary>
    /// <param name="email">Почта пользователя.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("dfdgdg")]
    [InlineData("@#$$#")]
    public async Task CreateNewClient_WithInvalidEmail_ReturnsBadRequest(string email)
    {
        // Arrange
        var data = getDataClients();
        data.Email = email;

        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным login.
    /// </summary>
    /// <param name="login">Логин.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateNewClient_WithInvalidLogin_ReturnsBadRequest(string login)
    {
        // Arrange
        var data = getDataClients();
        data.Login = login;

        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным password.
    /// </summary>
    /// <param name="password">Пароль.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123456dfsd")]
    [InlineData("fkkoe454ldlsl343lssl5")]
    public async Task CreateNewClient_WithInvalidPassword_ReturnsBadRequest(string password)
    {
        // Arrange
        var data = getDataClients();
        data.Password = password;

        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным birthdate.
    /// </summary>
    /// <param name="birthdate">Дата рождения пользователя.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("2020-12-03")]
    public async Task CreateNewClient_WithInvalidBirthDate_ReturnsBadRequest(string birthdate)
    {
        // Arrange
        var data = getDataClients();
        data.Birthdate = birthdate;

        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

    /// <summary>
    /// Создание пользователя с невалидным phonenumber.
    /// </summary>
    /// <param name="phonenumber"></param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("+67050-5")]
    [InlineData("+798756778980")]
    public async Task CreateNewClient_WithInvalidPhoneNumber_ReturnsBadRequest(string phonenumber)
    {
        // Arrange
        var data = getDataClients();
        data.PhoneNumber = phonenumber;

        // Act
        var сreateClientResponse = await ClientsRestClient.CreateClient(data);

        // Asserts
        Assert.Equal(HttpStatusCode.UnprocessableContent, сreateClientResponse.StatusCode);
    }

}
