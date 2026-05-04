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
    public async Task CheckState_ReturnsOk()
    {
        //Act
        var account = await CreateAndLockAccount();

        //Asserts
        Assert.Equal("Blocked", account.State);
    }

    /// <summary>
    /// Пополнение на заблокированный банковский счет.
    /// </summary>
    [Fact]
    public async Task AutoRefill_AfterLockAccount_ReturnsBadRequest()
    {
        //Arrange
        var account = await CreateAndLockAccount();

        //Act
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountRefill", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "Account", Value = $"[{account.Number}]" },
            new ParametrOperation { Identifier = "Amount", Value = "10" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData!.RequestId, body, accessToken);

        //Asserts
        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод с заблокированного счета на другой счет.
    /// </summary>
    [Fact]
    public async Task Transfer_WithLockAccountToAnotherAccount_ReturnsBadRequest()
    {
        //Arrange
        var account = await CreateAndLockAccount();

        //Act
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = $"[{account.Number}]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = "40837744138351246154" },
            new ParametrOperation { Identifier = "Amount", Value = "1" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData!.RequestId, body, accessToken);


        //Asserts
        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод с другого счета на заблокированный счет.
    /// </summary>
    [Fact]
    public async Task Transfer_WithAnotherAccountToLockAccount_ReturnsBadRequest()
    {
        //Arrange
        var account = await CreateAndLockAccount();

        //Act
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = "[40855720615481714118]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = $"{account.Number}" },
            new ParametrOperation { Identifier = "Amount", Value = "1" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData!.RequestId, body, accessToken);

        //Asserts
        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
    }
}
