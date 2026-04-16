using RestSharp;
using TestProjectIntern_n1.Core.ModelsData;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Клиент для получения токена аутентификации.
/// </summary>
public class AuthenticationRestClient : BaseRestClient
{
    /// <summary>
    /// Получение токена аутентификации.
    /// </summary>
    /// <param name="login">Логин зарегистрированного пользователя.</param>
    /// <param name="password">Пароль зарегистрированного пользователя.</param>
    /// <returns>Объект с токеном аутентификации.</returns>
    public async Task<RestResponse> GetAuthenticationToken(string? login, string? password)
    {
        var request = CreateBaseRequest("api/authorization/token", Method.Post);
        var data = new DataClients { Login = login, Password = password };
        request.AddJsonBody(data);

        return await Client.ExecuteAsync(request);
    }
}
