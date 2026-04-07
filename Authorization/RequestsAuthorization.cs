
using RestSharp;


namespace Authorization;

using System.Net;
using System.Text.Json;
using BaseSpaceRequest;
using Clients;

public class RequestsAuthorization
{
    private static BaseUrl request = new BaseUrl();
    private  RequestsClients ClientRequests;
    public RequestsAuthorization(RequestsClients requests)
    {
        ClientRequests = requests;
    }
    private RestClient client = request.Client;
   


    public async Task<RestResponse> UniversalGetRequestAfterAuthorization(string resource)
    {
        await ClientRequests.RequestForEndpointCreateClients();
        await ClientRequests.PostAuthentificationToken();
        // Console.WriteLine($"Token after auth: {ClientRequests.serializationDataClients?.AccessToken}");
       var requestPost = request.GenRequest(resource,Method.Get);
       
       
            if (ClientRequests.serializationDataClients?.AccessToken != null)
        {
            requestPost.AddHeader("Authorization", $"Bearer {ClientRequests.serializationDataClients.AccessToken}");
        }
        
        
      

        return await client.ExecuteAsync(requestPost);
    }
    public async Task<RestResponse> UniversalNegativeGetRequestAfterAuthorization(string resource, string accessToken)
    {
        await ClientRequests.RequestForEndpointCreateClients();
        await ClientRequests.PostAuthentificationToken();
        // Console.WriteLine($"Token after auth: {ClientRequests.serializationDataClients?.AccessToken}");
       var requestPost = request.GenRequest(resource,Method.Get);
       
       
            
            requestPost.AddHeader("Authorization", $"Bearer {accessToken}");
        
        
        
      

        return await client.ExecuteAsync(requestPost);
    }
}
