using RestSharp;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для блокировки аккаунта.
/// </summary>
public class LockAccountRestClient : BaseRestClient
{
    /// <summary>
    /// Блокировка банковского счета пользователя.
    /// </summary>
    /// <param name="accountId">Id счета.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> LockAccount<T>(T accountId, string accessToken)
    {
        var request = CreateBaseRequest($"api/accounts/lock/{accountId}", Method.Patch);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}
