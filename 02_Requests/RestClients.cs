using BaseSpaceRequests;
using RestSharp;

namespace Clients;

/// <summary>
/// Клиент для создания пользователей
/// </summary>
public class RestClients : BaseClient
{
    /// <summary>
    /// Запрос на создание пользователя
    /// </summary>
    /// <param name="data"> данные о пользователе </param>
    public async Task<RestResponse> CreateClient(DataClients data)
    {
        var createRequest = CreateBaseRequest("api/clients", Method.Put);

        createRequest.AddJsonBody(data);
        var response = await Client.ExecuteAsync(createRequest);

        return response;
    }
}
