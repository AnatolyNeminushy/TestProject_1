// Запрос на получение токена аунтетификации
// 
// *Вынес в отдельный модуль для удобства,приготся в других операциях


using RestSharp;

namespace BaseSpaceRequest;
public class AuthenticationToken
{
    public RestRequest RequestToObtainAuthenticationToken(string? login, string? password)
    {
        var request = new BaseUrl();
        var postRequest= request.GenRequest("api/authorization/token",Method.Post);
        postRequest.AddJsonBody(
        new DataClients
        {
            Login=login,
            Password=password,
        });
        return postRequest;
    }
}