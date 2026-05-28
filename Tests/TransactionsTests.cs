using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на получение транзакций.
/// </summary>
public class TransactionsTests : BaseTest
{
    /// <summary>
    /// Получение списка транзакций.
    /// </summary>
    [Fact]
    public async Task GetTransactions_ReturnsOk()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var getTransactionsResponse = await TransactionsRestClient.GetTransactions(accessToken);

        //Assert
        Assert.Equal(HttpStatusCode.OK, getTransactionsResponse.StatusCode);
    }

    /// <summary>
    /// Получение списка транзакций с идентификатором счета.
    /// </summary>
    [Fact]
    public async Task GetTransactions_WithAccountId_ReturnsOk()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var getTransactionsResponse = await TransactionsRestClient.GetTransactionsByAccountId(accessToken, 11705);

        //Assert
        Assert.Equal(HttpStatusCode.OK, getTransactionsResponse.StatusCode);
    }

    /// <summary>
    /// Получение информации о конкретной транзакции.
    /// </summary>
    [Fact]
    public async Task GetTransactions_WithTransactionId_ReturnsOk()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var getTransactionsResponse = await TransactionsRestClient.GetTransactionsByTransactionId(accessToken, 5500);

        //Assert
        Assert.Equal(HttpStatusCode.OK, getTransactionsResponse.StatusCode);
    }

    /// <summary>
    /// Получение списка транзакций с невалидным идентификатором счета.
    /// </summary>
    [Fact]
    public async Task GetTransactions_WithInvalidAccountId_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var getTransactionsResponse = await TransactionsRestClient.GetTransactionsByAccountId(accessToken, 0);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, getTransactionsResponse.StatusCode);
    }

    /// <summary>
    /// /// Получение информации о конкретной транзакции с невалидным идентификатором.
    /// </summary>
    [Fact]
    public async Task GetTransactions_WithInvalidTransactionId_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var getTransactionsResponse = await TransactionsRestClient.GetTransactionsByTransactionId(accessToken, 0);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, getTransactionsResponse.StatusCode);
    }
}
