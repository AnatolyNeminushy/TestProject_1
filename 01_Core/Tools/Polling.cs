using Clients;
using Operations;
using Polly;
using RestSharp;

namespace Tools;

/// <summary>
/// Инструмент для вызова повторяющихся запросов на сервер.
/// </summary>
public class Polling
{


    /// <summary>
    /// Получение баланса.
    /// </summary>
    /// <param name="arithmeticOperation">Тип арифмитической операции.</param>
    /// <param name="firstValue">Первое слагаемое.</param>
    /// <param name="secondValue">Второе слагамое.</param>
    /// <param name="token">Токен аутентификации.</param>
    /// <param name="account">Банковский счет пользователя.</param>
    /// <returns>Баланс счета.</returns>
    /// <exception cref="ArgumentException">Исключение, 
    /// если выбрана невалидная арифметическая операция.</exception>
    public static async Task<decimal> ForGetBalance(
        string arithmeticOperation,
        decimal firstValue, decimal secondValue, string token, string account)
    {
        var restClients = new RestClients();
        decimal expectedBalance;

        expectedBalance = arithmeticOperation switch
        {
            "addition" => firstValue + secondValue,
            "subtraction" => firstValue - secondValue,
            _ => throw new ArgumentException(
                "Выберите операцию из предложенного списка: addition, subtraction")
        };

        var policy = Policy<decimal>
            .Handle<Exception>()
            .OrResult(balance => balance != expectedBalance)
            .WaitAndRetryAsync(60, n => TimeSpan.FromSeconds(1));

        var valueAccountAfterOperation = await policy.ExecuteAsync(async () =>
        {
            var getAccountAfterAutorefillRequest =
                restClients
                    .CreateBaseRequest("api/accounts", Method.Get, token);
            var getAccountAfterAutorefillResponse =
                await restClients
                    .Client.ExecuteAsync(getAccountAfterAutorefillRequest);
            var getAccountAfterAutorefillData =
                JsonDeserializer
                    .DeserializeData<List<BankAccount>>(
                        getAccountAfterAutorefillResponse.Content);

            return getAccountAfterAutorefillData
                .FirstOrDefault(x => x.Number == account)!.Balance;
        });

        return valueAccountAfterOperation;
    }
}

