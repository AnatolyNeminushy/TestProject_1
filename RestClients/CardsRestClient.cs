using RestSharp;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для получения карт пользователя.
/// </summary>
public class CardsRestClient : BaseRestClient
{
    /// <summary>
    /// Получение карт клиента.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> GetCards(string accessToken)
    {
        var request = CreateBaseRequest("api/cards", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение заказов карт пользователя.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> GetCardsOrders(string accessToken)
    {
        var request = CreateBaseRequest("api/cards/orders", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}

