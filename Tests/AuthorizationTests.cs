using RestSharp;
using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на авторизацию.
/// </summary>
public class AuthorizationTests : BaseTest
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

        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var accountsResponse = await AccountsRestClient.GetAccount(accessToken);
        var cardsResponse = await CardsRestClient.GetCards(accessToken);
        var cardsOrdersResponse = await CardsRestClient.GetCardsOrders(accessToken);

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
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(login, password);

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
        // Act
        var accountsResponse = await AccountsRestClient.GetAccount(accessToken);
        var cardsResponse = await CardsRestClient.GetCards(accessToken);
        var cardsOrdersResponse = await CardsRestClient.GetCardsOrders(accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.Unauthorized, accountsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, cardsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, cardsOrdersResponse.StatusCode);
    }
}
