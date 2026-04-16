using RestSharp;
using System.Net;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Тесты на переводы на другие баноковские счета.
/// </summary>
public class TransferToAnotherAccountTests : BaseTest
{
    /// <summary>
    /// Перевод с текущего счета на счет другого пользователя.
    /// </summary>
    [Fact]
    public async Task Transfer_WithExistingDataClient_ReturnsOk()
    {
        // Arrange
        // Токен для текущего счета
        var authenticationCurrentAccountResponse = await AuthenticationRestClient.GetAuthenticationToken(
                    Login,
                    Password);
        var authenticationCurrentAccountData = JsonDeserializer.DeserializeData<DataClients>(authenticationCurrentAccountResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessCurrentAccountToken = authenticationCurrentAccountData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Токен для другого счета
        var authenticationAnotherAccountResponse = await AuthenticationRestClient.GetAuthenticationToken(
                    LoginAnotherAccount,
                    PasswordAnotherAccount);
        var authenticationAnotherAccountData = JsonDeserializer.DeserializeData<DataClients>(authenticationCurrentAccountResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessAnotherAccountToken = authenticationCurrentAccountData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        // Запросы текущего счета
        var getCurrentAccountBeforeTransferResponse = await AccountsRestClient.GetAccount(accessCurrentAccountToken);
        var getCurrentAccountBeforeTransferData = JsonDeserializer.DeserializeData<List<BankAccount>>(
                    getCurrentAccountBeforeTransferResponse.Content!);

        // Запросы другого счета
        var getAnotherAccountBeforeTransferResponse = await AccountsRestClient.GetAccount(accessAnotherAccountToken);
        var getAnotherAccountBeforeTransferData = JsonDeserializer.DeserializeData<List<BankAccount>>(
                    getAnotherAccountBeforeTransferResponse.Content!);

        var valueCurrentAccountBeforeTransfer = getCurrentAccountBeforeTransferData
                ?.FirstOrDefault(x => x.Number == "40843043375888642346")
                ?.Balance
                ?? throw new Exception("Не найден баланс текущего счета.");
        var valueAnotherAccountBeforeTransfer = getAnotherAccountBeforeTransferData
                ?.FirstOrDefault(x => x.Number == "40830755020207104405")
                ?.Balance
                ?? throw new Exception("Не найден баланс другого счета.");

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessCurrentAccountToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = "[40843043375888642346]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = "40830755020207104405" },
            new ParametrOperation { Identifier = "Amount", Value = "10" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData!.RequestId, body, accessCurrentAccountToken);
        var nextStepOperationData = JsonDeserializer.DeserializeData<InfoOperation>(nextStepOperationResponse.Content!);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(startOperationData.RequestId, accessCurrentAccountToken);
        var confirmedOperationData = JsonDeserializer.DeserializeData<InfoOperation>(confirmedOperationResponse.Content!);

        // Текущий счет
        var valueCurrentAccountAfterTransfer = await Polling.GetAccountAfterOperation(
            valueCurrentAccountBeforeTransfer - DifferenceAmount,
            accessCurrentAccountToken, "40843043375888642346");

        // Другой счет
        var valueAnotherAccountAfterTransfer = await Polling.GetAccountAfterOperation(
            valueCurrentAccountBeforeTransfer - DifferenceAmount,
            accessAnotherAccountToken, "40830755020207104405");

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

        Assert.Equal(valueCurrentAccountBeforeTransfer - DifferenceAmount, valueCurrentAccountAfterTransfer.Balance);
        Assert.Equal(valueAnotherAccountBeforeTransfer + DifferenceAmount, valueAnotherAccountAfterTransfer.Balance);
    }

    /// <summary>
    /// Перевод суммы с копейками.
    /// </summary>
    /// <param name="amount">Сумма.</param>
    [Theory]
    [InlineData("0.15")]
    [InlineData("5,1")]
    public async Task Transfer_WithPenny_ReturnsOk(string amount)
    {
        // Arrange
        // Токен для текущего счета
        var authenticationCurrentAccountResponse = await AuthenticationRestClient.GetAuthenticationToken(
                    Login, Password);
        var authenticationCurrentAccountData = JsonDeserializer.DeserializeData<DataClients>(authenticationCurrentAccountResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessCurrentAccountToken = authenticationCurrentAccountData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Токен для другого счета
        var authenticationAnotherAccountResponse = await AuthenticationRestClient.GetAuthenticationToken(
                    LoginAnotherAccount, PasswordAnotherAccount);
        var authenticationAnotherAccountData = JsonDeserializer.DeserializeData<DataClients>(authenticationCurrentAccountResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessAnotherAccountToken = authenticationCurrentAccountData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        // Запросы текущего счета
        var getCurrentAccountBeforeTransferResponse = await AccountsRestClient.GetAccount(accessCurrentAccountToken);
        var getCurrentAccountBeforeTransferData = JsonDeserializer.DeserializeData<List<BankAccount>>(
                    getCurrentAccountBeforeTransferResponse.Content!);

        // Запросы другого счета
        var getAnotherAccountBeforeTransferResponse = await AccountsRestClient.GetAccount(accessAnotherAccountToken);
        var getAnotherAccountBeforeTransferData = JsonDeserializer.DeserializeData<List<BankAccount>>(
                    getAnotherAccountBeforeTransferResponse.Content!);

        var valueCurrentAccountBeforeTransfer = getCurrentAccountBeforeTransferData
                ?.FirstOrDefault(x => x.Number == "40843043375888642346")?.Balance
                ?? throw new Exception("Не найден баланс текущего счета.");
        var valueAnotherAccountBeforeTransfer = getAnotherAccountBeforeTransferData
                ?.FirstOrDefault(x => x.Number == "40830755020207104405")?.Balance
                ?? throw new Exception("Не найден баланс другого счета.");

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessCurrentAccountToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = "[40843043375888642346]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = "40830755020207104405" },
            new ParametrOperation { Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(
            startOperationData!.RequestId, body, accessCurrentAccountToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(
                startOperationData.RequestId, accessCurrentAccountToken);

        // Текущий счет
        var valueCurrentAccountAfterTransfer = await Polling.GetAccountAfterOperation(
            valueCurrentAccountBeforeTransfer - DifferenceAmount,
            accessCurrentAccountToken, "40843043375888642346");

        // Другой счет
        var valueAnotherAccountAfterTransfer = await Polling.GetAccountAfterOperation(
            valueCurrentAccountBeforeTransfer - DifferenceAmount,
            accessAnotherAccountToken, "40830755020207104405");

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, nextStepOperationResponse.StatusCode);

        Assert.Equal(HttpStatusCode.OK, confirmedOperationResponse.StatusCode);

        Assert.Equal(valueCurrentAccountBeforeTransfer - DifferenceAmount, valueCurrentAccountAfterTransfer.Balance);
        Assert.Equal(valueAnotherAccountBeforeTransfer + DifferenceAmount, valueAnotherAccountAfterTransfer.Balance);
    }
    /// <summary>
    /// Перевод с невалидным кодом операции.
    /// </summary>
    /// <param name="operationCode">Код операции.</param>
    [Theory]
    [InlineData("Catffee")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("&*#&@-")]
    public async Task Transfer_WithInvalidOperationCode_ReturnsBadRequest(string operationCode)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!);
        var accessToken = authenticationData?.AccessToken!;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation(operationCode, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.BadRequest, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод с невалидным кодом операции состоящим из цифр.
    /// </summary>
    [Fact]
    public async Task Transfer_WithInvalidOperationNumberCode_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!);
        var accessToken = authenticationData?.AccessToken!;

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("1", accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.InternalServerError, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод на невалидные суммы.
    /// </summary>
    /// <param name="amount">Сумма денег.</param>
    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData(null)]
    public async Task Transfer_WithInvalidAmount_ReturnsBadRequest(
        string amount)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = "[40843043375888642346]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = "40830755020207104405" },
            new ParametrOperation { Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(
            startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(
            startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод на буквенную и символьную суммы.
    /// </summary>
    /// <param name="amount">Сумма денег.</param>
    [Theory]
    [InlineData("abc")]
    [InlineData("$&#(@))")]
    public async Task Transfer_WithLetterSymbolAmount_ReturnsBadRequest(
        string amount)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = "[40843043375888642346]" },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = "40830755020207104405" },
            new ParametrOperation { Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(
            startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(
            startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.InternalServerError, nextStepOperationResponse.StatusCode);
    }

    /// <summary>
    /// Переводы с невалидными данными счетов.
    /// </summary>
    /// <param name="currentAccount">Текущий счет.</param>
    /// <param name="anotherAccount">Другой счет.</param>
    [Theory]
    [InlineData("", "40830755020207104405")]
    [InlineData(null, "40830755020207104405")]
    [InlineData("[40843043375888642346]", "1234")]
    [InlineData("[40843043375888642346]", "")]
    [InlineData("[40843043375888642346]", null)]
    public async Task Transfer_WithInvalidAccount_ReturnsBadRequest(
        string currentAccount, string anotherAccount)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = currentAccount },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = anotherAccount },
            new ParametrOperation { Identifier = "Amount", Value = "10" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(
            startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(
            startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
    }

    /// <summary>
    /// Перевод с невалидного счета.
    /// </summary>
    /// <param name="currentAccount">Текущий счет.</param>
    /// <param name="anotherAccount">Другой счет.</param>
    [Theory]
    [InlineData("1234", "40830755020207104405")]
    [InlineData("[]", "40830755020207104405")]
    public async Task Transfer_WithInvalidCurrentAccount_ReturnsBadRequest(
        string currentAccount, string anotherAccount)
    {
        // Arrange
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(
                Login, Password);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content!)
            ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse = await OperationsRestClient.StartOperation("AccountTransfer", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "SourceAccount", Value = currentAccount },
            new ParametrOperation { Identifier = "ReceiverAccount", Value = anotherAccount },
            new ParametrOperation { Identifier = "Amount", Value = "10" },
        };

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(
            startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(
            startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.InternalServerError, nextStepOperationResponse.StatusCode);
    }
}
