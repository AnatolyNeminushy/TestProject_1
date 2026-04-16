using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на открытие банковского счета.             
/// </summary>
public class OpeningAnAccountTests : BaseTest
{
    /// <summary>
    /// Открытие банковского счета у зарегистрированного пользователя.
    /// </summary>
    [Theory]
    [InlineData("Текущий счёт", "Российский Рубль")]
    [InlineData("Накопительный счёт", "Доллар США")]
    // можно написать все 8 тестов, но особого смысла в них нет,
    // так как от них фукнционал никак не зависит и не меняется
    public async Task OpeningAnAccount_WithExistingDataClient_ReturnsOk(string accountType, string currency)
    {
        // Arrange
        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "AccountType", Value = accountType },
            new ParametrOperation { Identifier = "Currency", Value = currency },
            new ParametrOperation { Identifier = "LastName", Value = "Ткаченко" },
            new ParametrOperation { Identifier = "FirstName", Value = "Дмитрий" },
            new ParametrOperation { Identifier = "MiddleName", Value = "Андреевич" },
            new ParametrOperation { Identifier = "Birthdate", Value = "1998-12-06" },
        };

        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountOpen", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content);

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData.RequestId, body, accessToken);
        var nextStepOperationData = JsonDeserializer.DeserializeData<InfoOperation>(nextStepOperationResponse.Content);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(startOperationData.RequestId, accessToken);
        var confirmedOperationData = JsonDeserializer.DeserializeData<InfoOperation>(confirmedOperationResponse.Content);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, nextStepOperationResponse.StatusCode);
        Assert.False(nextStepOperationData.IsConfirmed);
        Assert.True(nextStepOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, confirmedOperationResponse.StatusCode);
        Assert.True(confirmedOperationData.IsConfirmed);
        Assert.True(confirmedOperationData.IsFinished);
    }

    /// <summary>
    /// Открытие банковского счета у зарегистрированного пользователя
    /// с невалидным кодом операции.
    /// </summary>
    /// <param name="operationCode">Код операции.</param>
    [Theory]
    [InlineData("HDiirn")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("&*#&@-")]
    public async Task OpeningAnAccount_WithInvalidOperationCode_ReturnsBadRequest(string operationCode)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation(operationCode, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Открытие банковского счета у зарегистрированного пользователя 
    /// с невалидным кодом операции состоящим из цифр.
    /// </summary>
    [Fact]
    public async Task OpeningAnAccount_WithInvalidOperationNumberCode_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("1", accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Открытие банковского счета у зарегистрированного пользователя 
    /// с невалидной валютой.
    /// </summary>
    [Fact]
    public async Task OpeningAnAccount_WithInvalidCurrency_ReturnsBadRequest()
    {
        // Arrange
        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "AccountType", Value = "Накопительный счёт" },
            new ParametrOperation { Identifier = "Currency", Value = "Белорусский Рубль" },
            new ParametrOperation { Identifier = "LastName", Value = "Ткаченко" },
            new ParametrOperation { Identifier = "FirstName", Value = "Дмитрий" },
            new ParametrOperation { Identifier = "MiddleName", Value = "Андреевич" },
            new ParametrOperation { Identifier = "Birthdate", Value = "1998-12-06" },
        };

        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountOpen", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content);

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData.RequestId, body, accessToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);

        Assert.Equal(HttpStatusCode.InternalServerError, confirmedOperationResponse.StatusCode);
    }
}
