using BaseSpaceRequests;
using Clients;
using Polly;
using RestSharp;
using System.Net;
using Tools;

namespace Operations;

/// <summary>
/// Тесты на пополнение счета.
/// </summary>
public class AccountRefillTests
{
    private RestClients restClients = new RestClients();
    private RestOperations restOperations = new RestOperations();
    private AuthenticationClient authenticationToken = new AuthenticationClient();

    const string login = "DimaFire";
    const string password = "Dima5678";

    /// <summary>
    /// Пополнение счета с валидными данными.
    /// </summary>
    [Fact]
    public async Task AccountRefill_WithExistingDataClient_ReturnsOk()
    {
        // Arrange
        decimal differenceAmount = 10.00m;

        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);

        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!)
                ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        var getAccountBeforeAutorefillRequest =
            restClients.CreateBaseRequest("api/accounts", Method.Get, accessToken);

        // Act
        var getAccountBeforeAutorefillResponse =
            await restClients
                .Client.ExecuteAsync(getAccountBeforeAutorefillRequest);
        var getAccountBeforeAutorefillData =
            JsonDeserializer
                .DeserializeData<List<BankAccount>>(
                    getAccountBeforeAutorefillResponse.Content!);

        var valueAccountBeforeAutorefill =
            getAccountBeforeAutorefillData
                ?.FirstOrDefault(x => x.Number == "40875518618438343578")?.Balance
                ?? throw new Exception("Не найден баланс счета."); ;

        var startOperationResponse =
            await restOperations.StartOperation("AccountRefill", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Account", Value = "[40875518618438343578]" },
            new ParametrOperation {
                Identifier = "Amount", Value = "10" },
        };

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(
                    startOperationData!.RequestId, body, accessToken);
        var nextStepOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(nextStepOperationResponse.Content!);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);
        var confirmedOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(confirmedOperationResponse.Content!);

        var valueAccountAfterAutorefill = await Polling.ForGetBalance(
            valueAccountBeforeAutorefill + differenceAmount,
            accessToken, "40875518618438343578");

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

        Assert.Equal(
            valueAccountBeforeAutorefill + differenceAmount,
            valueAccountAfterAutorefill);
    }

    /// <summary>
    /// Пополнение счета суммы с копейками.
    /// </summary>
    /// <param name="amount">Сумаа.</param>
    [Theory]
    [InlineData("0.15")]
    [InlineData("5,1")]
    public async Task AccountRefill_WithPenny_ReturnsOk(
        string amount)
    {
        // Arrange
        decimal differenceAmount = 10.00m;

        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);

        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!)
                ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        var getAccountBeforeAutorefillRequest =
            restClients.CreateBaseRequest("api/accounts", Method.Get, accessToken);

        // Act
        var getAccountBeforeAutorefillResponse =
            await restClients
                .Client.ExecuteAsync(getAccountBeforeAutorefillRequest);
        var getAccountBeforeAutorefillData =
            JsonDeserializer
                .DeserializeData<List<BankAccount>>(
                    getAccountBeforeAutorefillResponse.Content!);

        var valueAccountBeforeAutorefill =
            getAccountBeforeAutorefillData
                ?.FirstOrDefault(x => x.Number == "40875518618438343578")?.Balance
                ?? throw new Exception("Не найден баланс текущего счета."); ; ;

        var startOperationResponse =
            await restOperations.StartOperation("AccountRefill", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Account", Value = "[40875518618438343578]" },
            new ParametrOperation {
                Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(
                    startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);

        var valueAccountAfterAutorefill = await Polling.ForGetBalance(
            valueAccountBeforeAutorefill + differenceAmount,
            accessToken, "40875518618438343578");

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, nextStepOperationResponse.StatusCode);

    }

    /// <summary>
    /// Пополнение банковского счета у зарегистрированного пользователя
    /// с невалидным кодом операции.
    /// </summary>
    /// <param name="operationCode">Код операции.</param>
    [Theory]
    [InlineData("DogVlid")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("&*#&@-")]
    public async Task AccountRefill_WithInvalidOperationCode_ReturnsBadRequest(
        string operationCode)
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!);
        var accessToken = authenticationData?.AccessToken!;

        var getAccountBeforeAutorefillRequest =
            restClients
                .CreateBaseRequest("api/accounts", Method.Get, accessToken);

        // Act
        var startOperationResponse =
            await restOperations.StartOperation(operationCode, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.BadRequest, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Пополнение банковского счета у зарегистрированного пользователя 
    /// с невалидным кодом операции состоящим из цифр.
    /// </summary>
    /// <param name="operationCode">Код операции.</param>
    [Fact]
    public async Task AccountRefill_WithInvalidOperationNumberCode_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!);
        var accessToken = authenticationData?.AccessToken!;

        var getAccountBeforeAutorefillRequest =
            restClients
                .CreateBaseRequest("api/accounts", Method.Get, accessToken);

        // Act
        var getAccountBeforeAutorefillResponse =
            await restClients.Client
                .ExecuteAsync(getAccountBeforeAutorefillRequest);
        var getAccountBeforeAutorefillData =
            JsonDeserializer
                .DeserializeData<List<BankAccount>>(
                    getAccountBeforeAutorefillResponse.Content!);

        var startOperationResponse =
            await restOperations.StartOperation("1", accessToken);

        // Asserts
        Assert.Equal(
            HttpStatusCode.InternalServerError, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Пополнение банковского счета на невалидные суммы.
    /// </summary>
    /// <param name="amount">Сумма денег.</param>
    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData(null)]
    public async Task AccountRefill_WithInvalidAmount_ReturnsBadRequest(
        string amount)
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!)
                ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse =
            await restOperations.StartOperation("AccountRefill", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Account", Value = "[40875518618438343578]" },
            new ParametrOperation {
                Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(
            HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);
        Assert.Equal(
            HttpStatusCode.InternalServerError, confirmedOperationResponse.StatusCode);
    }

    /// <summary>
    /// Пополнение счета на буквенную и символьную суммы.
    /// </summary>
    /// <param name="amount">Сумма денег.</param>
    [Theory]
    [InlineData("abc")]
    [InlineData("$&#(@))")]
    public async Task AccountRefill_WithLetterSymbolAmount_ReturnsBadRequest(
        string amount)
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content!)
                ?? throw new Exception("Ошибка при десериализации.");
        var accessToken = authenticationData.AccessToken
            ?? throw new Exception("Ошибка при получении токена.");

        // Act
        var startOperationResponse =
            await restOperations.StartOperation("AccountRefill", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content!);

        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Account", Value = "[40875518618438343578]" },
            new ParametrOperation {
                Identifier = "Amount", Value = amount },
        };

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(startOperationData!.RequestId, body, accessToken);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);
        Console.WriteLine(nextStepOperationResponse.StatusCode);
        Assert.False(startOperationData.IsConfirmed);
        Assert.False(startOperationData.IsFinished);

        Assert.Equal(
            HttpStatusCode.InternalServerError, nextStepOperationResponse.StatusCode);
        Assert.Equal(
            HttpStatusCode.InternalServerError, confirmedOperationResponse.StatusCode);
    }
}
