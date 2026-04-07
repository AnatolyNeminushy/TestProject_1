
using RestSharp;


namespace Clients;

using System.Net;
using System.Text.Json;
using BaseSpaceRequest;


public class RequestsClients
{
    private static BaseUrl request = new BaseUrl();
    private RestClient client = request.Client;
    public DataClients? serializationDataClients;
    public async Task<RestResponse> RequestForEndpointCreateClients()
    {
        var unique = DateTime.Now.Ticks;
        var random = new Random();
        var phoneNumber = "+7"+ random.Next(100000000,1000000000).ToString();
        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(2005, 12, 31);
        var randomDate = start.AddDays(random.Next((end - start).Days));
        var formattedDate = randomDate.ToString("yyyy-MM-dd");

        var createRequest = request.GenRequest("api/clients", Method.Put);
        
        serializationDataClients = new DataClients
            {
                 PhoneNumber= phoneNumber,
                 Login=$"user{unique}",
                 Email=$"user{unique}@email.ru",
                 FirstName=$"name{unique}",
                 LastName=$"lastname{unique}",
                 MiddleName=$"middlenamfffffe{unique}",
                 Sex="Female",
                 Address=$"address{unique}",
                 Birthdate=formattedDate,
                 Password = $"password{unique}"

            };
        
        createRequest.AddJsonBody(serializationDataClients );
        

       return await client.ExecuteAsync(createRequest);

        


    }


     public async Task<RestResponse> RequestForEndpointGetClients()
    {
       
        var getRequest = request.GenRequest($"api/clients", Method.Get);
       if (serializationDataClients?.AccessToken != null)
        {
            getRequest.AddHeader("Authorization", $"Bearer {serializationDataClients.AccessToken}");
        }
        
       
       return await client.ExecuteAsync(getRequest);


    }
public async Task<RestResponse> RequestForEndpointNegativeCreateClients()
    {
        

        var createNegativeRequest = request.GenRequest("api/clients", Method.Put);
        var serializationNegativeDataClients = new DataClients();
        if(serializationDataClients != null)
        {
            serializationNegativeDataClients = serializationDataClients;
            
        }
        createNegativeRequest.AddJsonBody(serializationNegativeDataClients);
        


       return await client.ExecuteAsync(createNegativeRequest);

        


    }

   public async Task<RestResponse> PostAuthentificationToken()
{
    var requestToken = new AuthenticationToken();

    var postRequest = requestToken.RequestToObtainAuthenticationToken(
        serializationDataClients?.Login,
        serializationDataClients?.Password
    );

    var responseToken = await client.ExecuteAsync(postRequest);

    Console.WriteLine($"Auth status: {responseToken.StatusCode}");
    // Console.WriteLine($"Auth content: {responseToken.Content}");

    if (!string.IsNullOrWhiteSpace(responseToken.Content) && serializationDataClients != null)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var tokenData = JsonSerializer.Deserialize<DataClients>(responseToken.Content, options);

        if (tokenData != null && !string.IsNullOrEmpty(tokenData.AccessToken))
        {
            serializationDataClients.AccessToken = tokenData.AccessToken;
        }
    }

    return responseToken;
}
}
