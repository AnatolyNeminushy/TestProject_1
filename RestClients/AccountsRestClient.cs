using Polly;
using RestSharp;
using System.Security.Principal;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для получения счета пользователя.
/// </summary>
public class AccountsRestClient : BaseRestClient
{
    /// <summary>
    /// Получение счетов пользователя.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> GetAccounts(string accessToken)
    {
        var request = CreateBaseRequest("api/accounts", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение счета пользователя.
    /// </summary>
    /// <param name="accountId">Id счета.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> GetAccount(int accountId, string accessToken)
    {
        var request = CreateBaseRequest($"api/accounts/{accountId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение баланса.
    /// </summary>
    /// <param name="accountId">Id счета.</param>
    /// <param name="expectedBalance">Ожидаемый баланс.</param>
    /// <param name="token">Токен аутентификации.</param>
    /// <returns>Объект данных после завершения операции.</returns>
    public async Task<BankAccount> GetAccountAfterOperation(int accountId, decimal expectedBalance, string token)
    {
        var accountAfterOperation = await Policy<BankAccount>
            .Handle<Exception>()
            .OrResult(data => data.Balance != expectedBalance)
            .WaitAndRetryAsync(60, n => TimeSpan.FromSeconds(1))
            .ExecuteAsync(async () =>
            {
                var response = await GetAccount(accountId, token);

                return JsonDeserializer.DeserializeData<BankAccount>(response.Content);
            });

        return accountAfterOperation;
    }

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

    /// <summary>
    /// Разблокировка банковского счета пользователя.
    /// </summary>
    /// <param name="accountId">Id счета.</param>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Объект с данными пользователя.</returns>
    public async Task<RestResponse> UnlockAccount<T>(T accountId, string accessToken)
    {
        var request = CreateBaseRequest($"api/accounts/unlock/{accountId}", Method.Patch);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}

