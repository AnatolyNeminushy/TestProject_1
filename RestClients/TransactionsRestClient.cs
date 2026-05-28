using RestSharp;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для получения списка транзакций.
/// </summary>
public class TransactionsRestClient : BaseRestClient
{
    /// <summary>
    /// Получение списка транзакций всех счетов пользователя.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <returns>Список транзакций.</returns>
    public async Task<RestResponse> GetTransactions(string accessToken)
    {
        var request = CreateBaseRequest("api/transactions", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение списка транзакций определенного счета пользователя.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <param name="accountId">Идентификатор счета.</param>
    /// <returns>Список транзакций.</returns>
    public async Task<RestResponse> GetTransactionsByAccountId(string accessToken, int accountId)
    {
        var request = CreateBaseRequest($"api/transactions/byAccount/{accountId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }

    /// <summary>
    /// Получение информации о транзакции.
    /// </summary>
    /// <param name="accessToken">Токен аутентификации.</param>
    /// <param name="transactionId">Идентификатор транзакции.</param>
    /// <returns>Информация о транзакции.</returns>
    public async Task<RestResponse> GetTransactionsByTransactionId(string accessToken, int transactionId)
    {
        var request = CreateBaseRequest($"api/transactions/info/{transactionId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}
