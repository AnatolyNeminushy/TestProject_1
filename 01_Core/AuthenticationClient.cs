using RestSharp;

namespace BaseSpaceRequests;

/// <summary>
/// Клиент для получения токена аутентификации
/// </summary>
public class AuthenticationClient : BaseClient
{
    /// <summary>
    /// Запрос на получение токена аутентификации
    /// </summary>
    /// <param name="login">логин зарегистрированного пользователя</param>
    /// <param name="password">пароль зарегистрированного пользователя</param>
    /// <returns> RestResponse - response объект с токеном аутентификации</returns>
    public async Task<RestResponse> RequestToObtainAuthenticationToken(
        string? login,
        string? password
    )
    {
        var postRequest = CreateBaseRequest("api/authorization/token", Method.Post);
        var data = new DataClients { Login = login, Password = password };
        postRequest.AddJsonBody(data);

        return await Client.ExecuteAsync(postRequest);
    }
}
