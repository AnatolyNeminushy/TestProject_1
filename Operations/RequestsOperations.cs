namespace Operations;

using System.Text.Json;
using BaseSpaceRequest;
using Clients;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using RestSharp;
public class RequestOperations
{
     private static BaseUrl request = new BaseUrl();
    private RestClient client = request.Client;
    private RequestsClients requestsClients =  new RequestsClients();

    public OperationInfo putOperationInfo = new OperationInfo();
    public OperationInfo patchOperationInfo = new OperationInfo();
    public OperationInfo postOperationInfo = new OperationInfo();
    JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

    public async Task<RestResponse> UniversalPutRequestForEndpointOperations(string operationCode)
    {
        await requestsClients.RequestForEndpointCreateClients();
        await requestsClients.PostAuthentificationToken();
      
        var createRequest = request.GenRequest("api/operations", Method.Put);
         if (requestsClients.serializationDataClients?.AccessToken != null)
        {
           createRequest.AddHeader("Authorization", $"Bearer {requestsClients.serializationDataClients.AccessToken}");
        }


        createRequest.AddJsonBody(new
        {
            
                operationCode= operationCode
            
        });

        var responseRequest =await client.ExecuteAsync(createRequest);
          

        if (!string.IsNullOrWhiteSpace(responseRequest.Content))

        {
            


            var dataDeserialize= JsonSerializer.Deserialize<OperationInfo>(responseRequest.Content, options);
           
            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
        {
            putOperationInfo.RequestId = dataDeserialize.RequestId;
            putOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
            putOperationInfo.IsFinished = dataDeserialize.IsFinished;
            // operationInfo.Name = dataDeserialize.Name;
            // Console.WriteLine($"Nameeee{operationInfo.Name}");
        }
        }





       return responseRequest;

    }
    public async Task<RestResponse> UniversalPatchRequestForEndpointOperations(List<OperationInfo> body)
    {
        
        var createRequest = request.GenRequest("api/operations", Method.Patch);
       
         if (requestsClients.serializationDataClients?.AccessToken != null && putOperationInfo.RequestId!=0)
        {
           createRequest.AddHeader("Authorization", $"Bearer {requestsClients.serializationDataClients.AccessToken}");
            createRequest.AddQueryParameter("requestId", putOperationInfo.RequestId);
            
        }

        createRequest.AddJsonBody(body);

        var responseRequest = await client.ExecuteAsync(createRequest);

        if (!string.IsNullOrWhiteSpace(responseRequest.Content)){
        var dataDeserialize= JsonSerializer.Deserialize<OperationInfo>(responseRequest.Content, options);

           
            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
        {
            patchOperationInfo.RequestId= dataDeserialize.RequestId;
            patchOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
            patchOperationInfo.IsFinished = dataDeserialize.IsFinished;
            
        }}
       return responseRequest;

    }
     public async Task<RestResponse> UniversalPostRequestForEndpointOperations()
    {
        
        var createRequest = request.GenRequest("api/operations", Method.Post);
        if (requestsClients.serializationDataClients?.AccessToken != null && putOperationInfo.RequestId!=0)
        {
            createRequest.AddHeader("Authorization", $"Bearer {requestsClients.serializationDataClients.AccessToken}");
            createRequest.AddQueryParameter("requestId", putOperationInfo.RequestId);
            
        }
        var responseRequest = await client.ExecuteAsync(createRequest);

        if (!string.IsNullOrWhiteSpace(responseRequest.Content)){
        var dataDeserialize= JsonSerializer.Deserialize<OperationInfo>(responseRequest.Content, options);

           
            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
        {
            postOperationInfo.RequestId= dataDeserialize.RequestId;
            postOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
            postOperationInfo.IsFinished = dataDeserialize.IsFinished;
            
        }}
       return responseRequest;

    }
}