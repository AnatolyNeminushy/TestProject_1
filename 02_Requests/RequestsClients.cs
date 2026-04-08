using RestSharp;

namespace Clients;

using BaseSpaceRequest;

public class RequestsClients
{
    private static BaseUrl request = new BaseUrl();
    private RestClient client = request.Client;

    //
    // Создание пользователя
    public async Task<(RestResponse, DataClients)> CreateRequestForEndpointClients()
    {
        var unique = DateTime.Now.Ticks;
        var random = new Random();
        var phoneNumber = "+7" + random.Next(100000000, 1000000000).ToString();
        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(2005, 12, 31);
        var randomDate = start.AddDays(random.Next((end - start).Days));
        var formattedDate = randomDate.ToString("yyyy-MM-dd");

        var createRequest = request.GenRequest("api/clients", Method.Put);

        var data = new DataClients
        {
            PhoneNumber = phoneNumber,
            Login = $"user{unique}",
            Email = $"user{unique}@email.ru",
            FirstName = $"name{unique}",
            LastName = $"lastname{unique}",
            MiddleName = $"middlenamfffffe{unique}",
            Sex = "Female",
            Address = $"address{unique}",
            Birthdate = formattedDate,
            Password = $"password{unique}",
        };
        // Console.WriteLine(data.Login);

        createRequest.AddJsonBody(data);
        var response = await client.ExecuteAsync(createRequest);
        return (response, data);
    }

    // Получение данных о пользователе GET endpoint + BearerToken
    public async Task<RestResponse> GetRequestForEndpointClients(
        string endpoint,
        string accessToken
    )
    {
        var getRequest = request.GenRequest(endpoint, Method.Get);
        getRequest.AddHeader("Authorization", $"Bearer {accessToken}");

        return await client.ExecuteAsync(getRequest);
    }
}
