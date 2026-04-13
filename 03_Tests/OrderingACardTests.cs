using BaseSpaceRequests;
using Clients;
using System.Net;
using Tools;

namespace Operations;

/// <summary>
/// Тесты на заказ карты.
/// </summary>
public class OrderingACardTests
{
    private RestOperations restOperations = new RestOperations();
    private AuthenticationClient authenticationToken = new AuthenticationClient();

    private const string login = "DimaFire";
    private const string password = "Dima5678";

    /// <summary>
    /// Заказ карты с валидными параметрами.
    /// </summary>
    /// <param name="cardType">Тип карты.</param>
    /// <param name="paymentSystem">Платежная система.</param>
    [Theory]
    [InlineData("Дебетовая карта", "Mastercard")]
    [InlineData("Кредитная карта", "Visa")]
    // можно написать все 8 тестов, но особого смысла в них нет,
    // так как от них фукнционал никак не зависит и не меняется

    public async Task OprderingACard_WithExistingDataClient_ReturnsOk(
        string cardType, string paymentSystem)
    {
        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Product", Value = cardType },
            new ParametrOperation {
                Identifier = "ProgramType", Value = paymentSystem },
            new ParametrOperation {
                Identifier = "LastName", Value = "Ткаченко" },
            new ParametrOperation {
                Identifier = "FirstName", Value = "Дмитрий" },
            new ParametrOperation {
                Identifier = "MiddleName", Value = "Андреевич" },
            new ParametrOperation {
                Identifier = "Birthdate", Value = "1998-12-06" }
        };

        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse =
            await restOperations
                .StartOperation("CardOrder", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content);

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(startOperationData.RequestId, body, accessToken);
        var nextStepOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(nextStepOperationResponse.Content);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);
        var confirmedOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(confirmedOperationResponse.Content);

        // Assert
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
    /// Заказ карты с невалидным кодом операции.
    /// </summary>
    /// <param name="operationCode">код операции</param>
    [Theory]
    [InlineData("fege")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("&*#&@-")]
    public async Task OprderingACard_WithInvalidOperationCode_ReturnsBadRequest(
        string operationCode)
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse =
            await restOperations.StartOperation(operationCode, accessToken);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Заказ карты с невалидным числовым кодом операции.
    /// </summary>
    [Fact]
    public async Task OprderingACard_WithInvalidOperationNumberCode_ReturnsBadRequest()
    {
        // Arrange
        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse =
            await restOperations.StartOperation("1", accessToken);

        // Assert
        Assert.Equal(
            HttpStatusCode.InternalServerError, startOperationResponse.StatusCode);
    }

    /// <summary>
    /// Заказ карты с невалидной валютой.
    /// </summary>
    /// <param name="cardType">Тип карты.</param>
    /// <param name="paymentSystem">Платежная система.</param>
    [Theory]
    [InlineData("Золотая карта", "Mastercard")]
    [InlineData("Дебетовая карта", "Т-банк")]
    [InlineData("", "Mastercard")]
    [InlineData("Дебетовая карта", "")]
    public async Task OprderingACard_WithInvalidProductProgramType_ReturnsBadRequest(
        string cardType, string paymentSystem)
    {
        // Arrange
        var body = new List<ParametrOperation>()
        {
            new ParametrOperation {
                Identifier = "Product", Value = cardType },
            new ParametrOperation {
                Identifier = "ProgramType", Value = paymentSystem },
            new ParametrOperation {
                Identifier = "LastName", Value = "Ткаченко" },
            new ParametrOperation {
                Identifier = "FirstName", Value = "Дмитрий" },
            new ParametrOperation {
                Identifier = "MiddleName", Value = "Андреевич" },
            new ParametrOperation {
                Identifier = "Birthdate", Value = "1998-12-06" },
        };

        var authenticationResponse =
            await authenticationToken
                .RequestToObtainAuthenticationToken(login, password);
        var authenticationData =
            JsonDeserializer
                .DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        // Act
        var startOperationResponse =
            await restOperations
                .StartOperation("CardOrder", accessToken);
        var startOperationData =
            JsonDeserializer
                .DeserializeData<InfoOperation>(startOperationResponse.Content);

        var nextStepOperationResponse =
            await restOperations
                .NextStepOperation(startOperationData.RequestId, body, accessToken);

        var confirmedOperationResponse =
            await restOperations
                .ConfirmedOperation(startOperationData.RequestId, accessToken);

        // Asserts
        Assert.Equal(HttpStatusCode.OK, startOperationResponse.StatusCode);

        Assert.Equal(HttpStatusCode.BadRequest, nextStepOperationResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.InternalServerError, confirmedOperationResponse.StatusCode);
    }
}
