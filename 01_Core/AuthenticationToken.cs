// Запрос на получение токена аунтетификации
//
// *Вынес в отдельный модуль для удобства,приготся в других операциях

using RestSharp;

namespace BaseSpaceRequest;

public class AuthenticationToken
{
    public async Task<RestResponse> RequestToObtainAuthenticationToken(
        string? login,
        string? password
    )
    {
        var request = new BaseUrl();
        var client = request.Client;
        var postRequest = request.GenRequest("api/authorization/token", Method.Post);
        var data = new DataClients { Login = login, Password = password };
        postRequest.AddJsonBody(data);
        return await client.ExecuteAsync(postRequest);
    }
}
