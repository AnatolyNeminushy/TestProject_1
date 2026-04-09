using System.Net;
using System.Text.Json;
using BaseSpaceRequest;
using Clients;

namespace Operations;

//
public class TransferToAnotherAccount
{
    private RequestsClients requestsClients = new RequestsClients();
    private RequestOperations requestOperations = new RequestOperations();
    private AuthenticationToken authenticationToken = new AuthenticationToken();
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    [Theory]
    // [InlineData("AccountRefill", "1000")]
    // [InlineData("AccountRefill", "0")]
    [InlineData("AccountTransfer", "10", 10.00)]
    // [InlineData("AccountRefill","Кредитная карта","Visa","lastname639111196098143429","name639111196098143429","middlenamfffffe639111196098143429","1971-12-18")]
    // можно написать все 8 тестов, но особого смысла в них нет, так как от них фукнционал никак не зависит и не меняется

    public async Task AccountRefill(string operationCode, string amount, decimal differenceAmount)
    {
        // Arrange
        var login = "DimaFire";
        var password = "Dima5678";
        var authToken = await authenticationToken.RequestToObtainAuthenticationToken(
            login,
            password
        );
        Assert.NotNull(authToken.Content);
        var dataAuthTokenDeserialize = JsonSerializer.Deserialize<DataClients>(
            authToken.Content,
            options
        );
        Assert.NotNull(dataAuthTokenDeserialize);
        var accessToken = dataAuthTokenDeserialize.AccessToken;

        // Act
        var responseGetClientBefore = await requestsClients.GetRequestForEndpointClients(
            "api/accounts",
            accessToken
        );
        Assert.NotNull(responseGetClientBefore.Content);
        var getDataBeforeCreateClientDeserialize = JsonSerializer.Deserialize<List<AccountInfo>>(
            responseGetClientBefore.Content,
            options
        );

        Assert.NotNull(accessToken);
        await Task.Delay(5000);
        var (putRequest, putData) =
            await requestOperations.UniversalPutRequestForEndpointOperations(
                operationCode,
                accessToken
            );
        // Console.WriteLine($"Value 0 {putData.StepParams[0].Values[0]}");
        // Console.WriteLine($"Value 1 {putData.StepParams[0].Values[1]}");
        Assert.NotNull(putData.StepParams);
        var neededAccountNumber = "40838044348266100636";

        var accountValue = putData
            .StepParams.First(x => x.Identifier == "SourceAccount")
            .Values?.FirstOrDefault(x => x.Contains(neededAccountNumber));

        Assert.NotNull(accountValue);

        var body = new List<OperationInfo>()
        {
            new OperationInfo { Identifier = "SourceAccount", Value = accountValue },
            new OperationInfo { Identifier = "ReceiverAccount", Value = "40873288080484314011" },
            new OperationInfo { Identifier = "Amount", Value = amount },
            new OperationInfo { Identifier = "Comment", Value = "Hi" },
        };

        var (patchRequest, patchData) =
            await requestOperations.UniversalPatchRequestForEndpointOperations(body, accessToken);
        var (postRequest, postData) =
            await requestOperations.UniversalPostRequestForEndpointOperations(
                accessToken,
                putData.RequestId
            );

        // put
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PUT_REQUEST] Status Code:{putRequest.StatusCode}");
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PUT_REQUEST] RequestId:{putData.RequestId}");
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PUT_REQUEST] IsConfirmed:{putData.IsConfirmed}");
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PUT_REQUEST] IsFinished:{putData.IsFinished}");
        // patch
        Console.WriteLine(
            $"[TRANSFER_TO_ACCOUNT_PATCH_REQUEST] Status Code:{patchRequest.StatusCode}"
        );
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PATCH_REQUEST] RequestId:{patchData.RequestId}");
        Console.WriteLine(
            $"[TRANSFER_TO_ACCOUNT_PATCH_REQUEST] IsConfirmed:{patchData.IsConfirmed}"
        );
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_PATCH_REQUEST] IsFinished:{patchData.IsFinished}");
        // post
        Console.WriteLine(
            $"[TRANSFER_TO_ACCOUNT_POST_REQUEST] Status Code:{postRequest.StatusCode}"
        );
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_POST_REQUEST] RequestId:{postData.RequestId}");
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_POST_REQUEST] IsConfirmed:{postData.IsConfirmed}");
        Console.WriteLine($"[TRANSFER_TO_ACCOUNT_POST_REQUEST] IsFinished:{postData.IsFinished}");
        Console.WriteLine(accessToken);
        //
        await Task.Delay(5000);
        var beforeAccount = getDataBeforeCreateClientDeserialize
            .FirstOrDefault(x => x.Number == neededAccountNumber)
            ?.Balance;
        var beforeBalance = Convert.ToDecimal(beforeAccount);
        Console.WriteLine($"[BEFORE_TRANSFER]:{accountValue}");

        var responseGetClient = await requestsClients.GetRequestForEndpointClients(
            "api/accounts",
            accessToken
        );
        Assert.NotNull(responseGetClient.Content);
        var getDataAfterCreateClientDeserialize = JsonSerializer.Deserialize<List<AccountInfo>>(
            responseGetClient.Content,
            options
        );
        var afterAccount = getDataAfterCreateClientDeserialize
            .FirstOrDefault(x => x.Number == neededAccountNumber)
            ?.Balance;
        Console.WriteLine($"[AFTER_TRANSFER]:{afterAccount}");
        // Console.WriteLine(beforeBalance + differenceAmount);
        // Console.WriteLine(afterAccount);

        Assert.Equal(HttpStatusCode.OK, putRequest.StatusCode);
        Assert.False(putData.IsConfirmed);
        Assert.False(putData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, patchRequest.StatusCode);
        Assert.False(patchData.IsConfirmed);
        Assert.True(patchData.IsFinished);

        Assert.Equal(HttpStatusCode.OK, postRequest.StatusCode);
        Assert.True(postData.IsConfirmed);
        Assert.True(postData.IsFinished);

        // // Asserts
        // // PUT
        // var putRequest = await requestOperations.UniversalPutRequestForEndpointOperations(
        //     operationCode
        // );
        // Console.WriteLine($"PutAccountRefillContent: {putRequest.Content}");
        // Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        // Assert.NotEqual(0, requestOperations.putOperationInfo.RequestId);
        // Assert.Equal(HttpStatusCode.OK, putRequest.StatusCode);
        // Assert.False(requestOperations.putOperationInfo.IsConfirmed);
        // Assert.False(requestOperations.putOperationInfo.IsFinished);

        // // PATCH
        // var patchRequest = await requestOperations.UniversalPatchRequestForEndpointOperations(body);
        // Console.WriteLine($"PatchAccountRefillStatus: {patchRequest.StatusCode}");
        // Console.WriteLine($"PatchAccountRefillContent: {patchRequest.Content}");
        // Console.WriteLine($"PatchAccountRefillStatus: {patchRequest.StatusCode}");
        // // Assert.Equal(HttpStatusCode.OK, patchRequest.StatusCode);
        // // Assert.False(requestOperations.patchOperationInfo.IsConfirmed);
        // // Assert.True(requestOperations.patchOperationInfo.IsFinished);

        // // // POST
        // // var postRequest = await requestOperations.UniversalPostRequestForEndpointOperations();
        // // Console.WriteLine($"PostOrderingACardContent: {postRequest.Content}");
        // // Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        // // Assert.NotEqual(0,requestOperations.postOperationInfo.RequestId);
        // // Assert.Equal(HttpStatusCode.OK, postRequest.StatusCode);
        // // Assert.True(requestOperations.postOperationInfo.IsConfirmed);
        // // Assert.True(requestOperations.postOperationInfo.IsFinished);
    }
}
