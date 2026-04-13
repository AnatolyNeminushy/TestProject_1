using RestSharp;
using System.Text;

namespace BaseSpaceRequests;

/// <summary>
/// Базовый клиент для запросов
/// </summary>
public class BaseClient
{
    public BaseClient()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var options = new RestClientOptions("https://itester.online/") { Encoding = Encoding.UTF8 };
        Client = new RestClient(options);
    }

    public readonly RestClient Client;

    /// <summary>
    /// Создание базового запроса
    /// </summary>
    /// <param name="resource">endpoint запроса</param>
    /// <param name="method">http-метод запроса</param>
    /// <returns>настроенный RestRequest объект</returns>
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

