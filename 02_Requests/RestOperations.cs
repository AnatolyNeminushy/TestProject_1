using BaseSpaceRequests;
using RestSharp;

namespace Operations;

/// <summary>
/// Клиент для совершения операций: создание счета, пополнения счета, заказа карты и перевода на другой счет
/// </summary>
public class RestOperations : BaseClient
{
    /// <summary>
    /// Запрос на отправление кода операции
    /// </summary>
    /// <param name="operationCode"> код операции </param>
    /// <param name="accessToken"> токен аутентификации </param>
    /// <returns> response объект с информацией об операции с параметрами </returns>
    public async Task<RestResponse> StartOperation(string operationCode, string accessToken)
    {
        var startRequest = CreateBaseRequest("api/operations", Method.Put);

        startRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        startRequest.AddJsonBody(new { operationCode = operationCode });

        var startResponse = await Client.ExecuteAsync(startRequest);

        return startResponse;
    }

    /// <summary>
    /// Запрос на отправку данных об операции
    /// </summary>
    /// <param name="requestId"> код запроса операции </param>
    /// <param name="body"> тело запроса </param>
    /// <param name="accessToken"> токен аутентификации </param>
    /// <returns> response объект с информацией об операции без параметров </returns>
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
    /// Запрос на подтверждение операции
    /// </summary>
    /// <param name="requestId"> код запроса операции </param>
    /// <param name="accessToken"> токен аутентификации </param>
    /// <returns> response объект с информацией об операции без параметров </returns>
    public async Task<RestResponse> ConfirmedOperation(int requestId, string accessToken)
    {
        var confirmedRequest = CreateBaseRequest("api/operations", Method.Post);

        confirmedRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        confirmedRequest.AddQueryParameter("requestId", requestId);

        var confirmedResponse = await Client.ExecuteAsync(confirmedRequest);

        return confirmedResponse;
    }
}
