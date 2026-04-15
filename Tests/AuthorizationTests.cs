using RestSharp;
using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на авторизацию.
/// </summary>
public class AuthorizationTests : BaseTests
{
    /// <summary>
    /// Авторизация с валидными данными пользователя.
    /// </summary>
    [Fact]
    public async Task AuthorizationClient_WithExistingDataClient_ReturnsOk()
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
        //

        var authenticationResponse = await AuthenticationRestClient.RequestToObtainAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var accountsRequest = ClientsRestClient.CreateBaseRequest("api/accounts", Method.Get, accessToken);
        var cardsRequest = ClientsRestClient.CreateBaseRequest("api/cards", Method.Get, accessToken);
        var cardsOrdersRequest = ClientsRestClient.CreateBaseRequest("api/cards/orders", Method.Get, accessToken);

        // Act
        var accountsResponse = await ClientsRestClient.Client.ExecuteAsync(accountsRequest);
        var cardsResponse = await ClientsRestClient.Client.ExecuteAsync(cardsRequest);
        var cardsOrdersResponse = await ClientsRestClient.Client.ExecuteAsync(cardsOrdersRequest);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, accountsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cardsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cardsOrdersResponse.StatusCode);
    }

    /// <summary>
    /// Получение токена с невалидными данными пользователя(логин и пароль).
    /// </summary>
    [Fact]
    public async Task AuthorizationClient_WithNonExisteningDataClient_ReturnsOk()
    {
        // Arrange
        var login = "DimaWater";
        var password = "Dima5678";

        // Act
        var authenticationResponse = await AuthenticationRestClient.RequestToObtainAuthenticationToken(login, password);

        // Asserts
        Assert.Equal(HttpStatusCode.BadRequest, authenticationResponse.StatusCode);
    }

    /// <summary>
    /// Авторизация с невалидными данными пользователя(токен аутентификации).
    /// </summary>
    [Theory]
    [InlineData("repfppdprgrp")]
    [InlineData("")]
    public async Task AuthorizationClient_WithNonExistentAccessToken_ReturnsUnauthorized(string accessToken)
    {
        // Arrange
        var accountsRequest = ClientsRestClient.CreateBaseRequest("api/accounts", Method.Get, accessToken);
        var cardsRequest = ClientsRestClient.CreateBaseRequest("api/cards", Method.Get, accessToken);
        var cardsOrdersRequest = ClientsRestClient.CreateBaseRequest("api/cards/orders", Method.Get, accessToken);

        // Act
        var accountsResponse = await ClientsRestClient.Client.ExecuteAsync(accountsRequest);
        var cardsResponse = await ClientsRestClient.Client.ExecuteAsync(cardsRequest);
        var cardsOrdersResponse = await ClientsRestClient.Client.ExecuteAsync(cardsOrdersRequest);

        // Asserts
        Assert.Equal(HttpStatusCode.Unauthorized, accountsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, cardsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, cardsOrdersResponse.StatusCode);
    }
}
