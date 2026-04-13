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
        var putRequest = CreateBaseRequest("api/operations", Method.Put);

        putRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        putRequest.AddJsonBody(new { operationCode = operationCode });

        var responseRequest = await Client.ExecuteAsync(putRequest);

        return responseRequest;
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
        var patchRequest = CreateBaseRequest("api/operations", Method.Patch);

        patchRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        patchRequest.AddQueryParameter("requestId", requestId);
        patchRequest.AddJsonBody(body);

        var responseRequest = await Client.ExecuteAsync(patchRequest);

        return responseRequest;
    }

    /// <summary>
    /// Запрос на подтверждение операции
    /// </summary>
    /// <param name="requestId"> код запроса операции </param>
    /// <param name="accessToken"> токен аутентификации </param>
    /// <returns> response объект с информацией об операции без параметров </returns>
    public async Task<RestResponse> ConfirmedOperation(int requestId, string accessToken)
    {
        var createRequest = CreateBaseRequest("api/operations", Method.Post);

        createRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        createRequest.AddQueryParameter("requestId", requestId);

        var responseRequest = await Client.ExecuteAsync(createRequest);

        return responseRequest;
    }
}
