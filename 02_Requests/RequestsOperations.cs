namespace Operations;

using System.Text.Json;
using BaseSpaceRequest;
using RestSharp;

public class RequestOperations
{
    private static BaseUrl request = new BaseUrl();
    private RestClient client = request.Client;
    public OperationInfo putOperationInfo = new OperationInfo();
    public OperationInfo patchOperationInfo = new OperationInfo();
    public OperationInfo postOperationInfo = new OperationInfo();
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    // private RequestsClients requestsClients = new RequestsClients();

    public async Task<(RestResponse, OperationInfo)> UniversalPutRequestForEndpointOperations(
        string operationCode,
        string accessToken
    )
    {
        var putRequest = request.GenRequest("api/operations", Method.Put);

        putRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        putRequest.AddJsonBody(new { operationCode = operationCode });

        var responseRequest = await client.ExecuteAsync(putRequest);

        var dataDeserialize = new OperationInfo();
        if (!string.IsNullOrWhiteSpace(responseRequest.Content))
        {
            dataDeserialize = JsonSerializer.Deserialize<OperationInfo>(
                responseRequest.Content,
                options
            );

            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
            {
                putOperationInfo.RequestId = dataDeserialize.RequestId;
                putOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
                putOperationInfo.IsFinished = dataDeserialize.IsFinished;
            }
        }

        return (responseRequest, dataDeserialize);
    }

    public async Task<(RestResponse, OperationInfo)> UniversalPatchRequestForEndpointOperations(
        List<OperationInfo> body,
        string accessToken
    )
    {
        var patchRequest = request.GenRequest("api/operations", Method.Patch);

        patchRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        patchRequest.AddQueryParameter("requestId", putOperationInfo.RequestId);
        patchRequest.AddJsonBody(body);

        var responseRequest = await client.ExecuteAsync(patchRequest);
        var dataDeserialize = new OperationInfo();
        if (!string.IsNullOrWhiteSpace(responseRequest.Content))
        {
            dataDeserialize = JsonSerializer.Deserialize<OperationInfo>(
                responseRequest.Content,
                options
            );

            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
            {
                patchOperationInfo.RequestId = dataDeserialize.RequestId;
                patchOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
                patchOperationInfo.IsFinished = dataDeserialize.IsFinished;
            }
        }
        return (responseRequest, dataDeserialize);
    }

    public async Task<(RestResponse, OperationInfo)> UniversalPostRequestForEndpointOperations(
        string accessToken,
        int requestId
    )
    {
        var createRequest = request.GenRequest("api/operations", Method.Post);

        createRequest.AddHeader("Authorization", $"Bearer {accessToken}");
        createRequest.AddQueryParameter("requestId", requestId);

        var responseRequest = await client.ExecuteAsync(createRequest);
        var dataDeserialize = new OperationInfo();
        if (!string.IsNullOrWhiteSpace(responseRequest.Content))
        {
            dataDeserialize = JsonSerializer.Deserialize<OperationInfo>(
                responseRequest.Content,
                options
            );

            if (dataDeserialize != null && dataDeserialize.RequestId != 0)
            {
                postOperationInfo.RequestId = dataDeserialize.RequestId;
                postOperationInfo.IsConfirmed = dataDeserialize.IsConfirmed;
                postOperationInfo.IsFinished = dataDeserialize.IsFinished;
            }
        }
        return (responseRequest, dataDeserialize);
    }
}
