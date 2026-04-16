using RestSharp;
using TestProjectIntern_n1.Core.ModelsData;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для создания пользователей.
/// </summary>
public class ClientsRestClient : BaseRestClient
{
    /// <summary>
    /// Создание пользователя.
    /// </summary>
    /// <param name="data">Данные о пользователе.</param>
    public async Task<RestResponse> CreateClient(DataClients data)
    {
        var createRequest = CreateBaseRequest("api/clients", Method.Put);

        createRequest.AddJsonBody(data);
        var response = await Client.ExecuteAsync(createRequest);

        return response;
    }

    /// <summary>
    /// Получние данных о пользователе.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<RestResponse> GetClient(string accessToken)
    {
        var request = CreateBaseRequest("api/clients", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}
