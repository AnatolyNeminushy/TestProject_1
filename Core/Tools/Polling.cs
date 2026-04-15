using Polly;
using RestSharp;
using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Core.Tools;
// TODO переименовать везде

/// <summary>
/// Инструмент для вызова повторяющихся запросов на сервер.
/// </summary>
public static class Polling
{
    /// <summary>
    /// Получение баланса.
    /// </summary>
    /// <param name="expectedBalance">Ожидаемый баланс.</param>
    /// <param name="token">Токен аутентификации.</param>
    /// <param name="account">Банковский счет пользователя.</param>
    /// <returns>Баланс счета.</returns>
    public static async Task<BankAccount> ForGetBalance(
        decimal expectedBalance, string token, string account)
    {
        var restClients = new ClientsRestClient();

        var request = restClients.CreateBaseRequest("api/accounts", Method.Get, token);

        var accountAfterOperation = await Policy<List<BankAccount>>
            .Handle<Exception>()
            .OrResult(data => data.FirstOrDefault(x => x.Number == account)!.Balance != expectedBalance)
            .WaitAndRetryAsync(60, n => TimeSpan.FromSeconds(1))
            .ExecuteAsync(async () =>
        {
            var response = await restClients.Client.ExecuteAsync(request);

            return JsonDeserializer.DeserializeData<List<BankAccount>>(response.Content);
        });

        return accountAfterOperation.FirstOrDefault(x => x.Number == account);
    }
}

