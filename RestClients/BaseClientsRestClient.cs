using RestSharp;
using System.Text;

namespace TestProjectIntern_n1.RestClients;

/// <summary>
/// Базовый клиент для запросов.
/// </summary>
public class BaseClientsRestClient
{
    public BaseClientsRestClient()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var options = new RestClientOptions("https://itester.online/") { Encoding = Encoding.UTF8 };
        Client = new RestClient(options);
    }

    public readonly RestClient Client;

    /// <summary>
    /// Создание базового запроса.
    /// </summary>
    /// <param name="resource">Endpoint запроса.</param>
    /// <param name="method">Http-метод запроса.</param>
    /// <returns>Настроенный запрос.</returns>
    public RestRequest CreateBaseRequest(string resource, Method method, string accessToken = null)
    {
        var request = new RestRequest(resource, method);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Accept-Charset", "utf-8");
        request.AddCookie("X-iTester-Access", "2ad3f9a0ee6b486db902ade89d6850ff");
        if (accessToken != null)
        {
            request.AddHeader("Authorization", $"Bearer {accessToken}");
        }

        return request;
    }
}

