// Базовый класс запроса на сервер
// 

using RestSharp;
using System.Text;
namespace BaseSpaceRequest;

public class BaseUrl
{
    public BaseUrl()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var options = new RestClientOptions("https://itester.online/")
        {
            Encoding = Encoding.UTF8
        };
        Client = new RestClient(options);
    }

    public readonly RestClient Client;

    public RestRequest GenRequest(string resource, Method method)
    {
        
        var request = new RestRequest(resource, method);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Accept-Charset", "utf-8");
        request.AddCookie("X-iTester-Access", "2ad3f9a0ee6b486db902ade89d6850ff");
        return request;
       
    }
}
