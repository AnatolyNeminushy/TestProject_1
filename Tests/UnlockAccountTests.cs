using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на разблокировку банковского счета пользователя.
/// </summary>
public class UnlockAccountTests : BaseTest
{
    /// <summary>
    /// Разблокировка банковского счета пользователя с валидным AccountId.
    /// </summary>
    [Fact]
    public async Task CheckState_ReturnsOk()
    {
        //Arrange
        var account = await CreateAndLockAccount();

        //Act
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var unlockAccountResponse = await AccountsRestClient.UnlockAccount(account.Id, accessToken);

        var getAccountResponse = await AccountsRestClient.GetAccount(account.Id, accessToken);
        var getAccountData = JsonDeserializer.DeserializeData<BankAccount>(getAccountResponse.Content);

        //Asserts
        Assert.Equal("Active", getAccountData.State);
    }

    /// <summary>
    /// Пополнение на разблокированный банковский счет.
    /// </summary>
    [Fact]
    public async Task AutoRefill_ToUnlockAccount_ReturnsOk()
    {
        //Arrange
        var account = await CreateAndLockAccount();

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "Account", Value = $"[{account.Number}]" },
            new ParametrOperation { Identifier = "Amount", Value = "10" },
        };

        // Act
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var unlockAccountResponse = await AccountsRestClient.UnlockAccount(account.Id, accessToken);

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountRefill", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData!.RequestId, body, accessToken);
        var nextStepOperationData = JsonDeserializer.DeserializeData<InfoOperation>(nextStepOperationResponse.Content!);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(startOperationData.RequestId, accessToken);
        var confirmedOperationData = JsonDeserializer.DeserializeData<InfoOperation>(confirmedOperationResponse.Content!);

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
}
