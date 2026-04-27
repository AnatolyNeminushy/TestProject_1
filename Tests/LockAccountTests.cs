
using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на блокировку банковского счета пользователя.
/// </summary>
public class LockAccountTests : BaseTest
{
    /// <summary>
    /// Блокировка банковского счета пользователя с валидным AccountId.
    /// </summary>
    [Fact]
    public async Task LockAccount_WithValidAccountId_ReturnsOk()
    {
        //Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var lockAccountResponse = await LockAccountRestClient.LockAccount(11705, accessToken);
        var lockAccountData = JsonDeserializer.DeserializeData<BankAccount>(lockAccountResponse.Content);

        //Asserts
        Assert.Equal(HttpStatusCode.OK, lockAccountResponse.StatusCode);
        Assert.Equal("Blocked", lockAccountData.State);
    }

    /// <summary>
    /// Блокировка банковского счета пользователя с невалидным AccountId.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(1)]
    [InlineData("#@^#&")]
    public async Task LockAccount_WithInvalidAccountId_ReturnsBadRequest<T>(T accountId)
    {
        //Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        //Act
        var lockAccountResponse = await LockAccountRestClient.LockAccount(accountId, accessToken);

        //Asserts
        Assert.Equal(HttpStatusCode.BadRequest, lockAccountResponse.StatusCode);
    }
}
