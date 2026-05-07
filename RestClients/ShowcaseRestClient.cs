using RestSharp;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для возвращения доступных продуктов.
/// </summary>
public class ShowcaseRestClient : BaseRestClient
{
    /// <summary>
    /// Получение информации о доступных продуктах.
    /// </summary>
    /// <param name="accessToken">Токен авторизации.</param>
    /// <returns>Объект с информацией о доступных продуктах.</returns>
    public async Task<RestResponse> GetShowcaseProducts()
    {
        var request = CreateBaseRequest("api/showcase/products", Method.Get);

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение информации о доступном продукте по его id.
    /// </summary>
    /// <param name="accessToken">Токен авторизации.</param>
    /// <param name="productId">Id продукта.</param>
    /// <returns>Объект с информацией о доступном продукте.</returns>
    public async Task<RestResponse> GetShowcaseProduct(int productId)
    {
        var request = CreateBaseRequest($"api/showcase/product/{productId}", Method.Get);

        return await Client.ExecuteAsync(request);
    }
}


