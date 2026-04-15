using RestSharp;
using TestProjectIntern_n1.Core.ModelsData;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для совершения операций: создание счета, пополнения счета, заказа карты и перевода на другой счет.
/// </summary>
public class OperationsRestClient : BaseClientsRestClient
{
    /// <summary>
    /// Отправление кода кода операции.
    /// </summary>
    /// <param name="operationCode">Код операции.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с информацией об операции.</returns>
    public async Task<RestResponse> StartOperation(string operationCode, string accessToken)
    {
        var startRequest = CreateBaseRequest("api/operations", Method.Put);

        startRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        startRequest.AddJsonBody(new { operationCode = operationCode });

        var startResponse = await Client.ExecuteAsync(startRequest);

        return startResponse;
    }

    /// <summary>
    /// Отправление данных об операции.
    /// </summary>
    /// <param name="requestId">Id запроса операции.</param>
    /// <param name="body">Тело запроса.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с информацией об операции.</returns>
    public async Task<RestResponse> NextStepOperation(int requestId, List<ParametrOperation> body, string accessToken)
    {
        var nextStepRequest = CreateBaseRequest("api/operations", Method.Patch);

        nextStepRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        nextStepRequest.AddQueryParameter("requestId", requestId);
        nextStepRequest.AddJsonBody(body);

        var nextStepResponse = await Client.ExecuteAsync(nextStepRequest);

        return nextStepResponse;
    }

    /// <summary>
    /// Подтверждение операции.
    /// </summary>
    /// <param name="requestId">Код запроса операции.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с информацией об операции.</returns>
    public async Task<RestResponse> ConfirmedOperation(int requestId, string accessToken)
    {
        var confirmedRequest = CreateBaseRequest("api/operations", Method.Post);

        confirmedRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        confirmedRequest.AddQueryParameter("requestId", requestId);

        var confirmedResponse = await Client.ExecuteAsync(confirmedRequest);

        return confirmedResponse;
    }
}
