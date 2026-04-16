using RestSharp;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для получения счета пользователя.
/// </summary>
public class AccountsRestClient : BaseRestClient
{
    /// <summary>
    /// Получение счета пользователя.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> GetAccount(string accessToken)
    {
        var request = CreateBaseRequest("api/accounts", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}

