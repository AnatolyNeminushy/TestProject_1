using System.Net;
using System.Text.Json;
using BaseSpaceRequest;
using Clients;

namespace Operations;

//
public class OrderingACardTests
{
    private RequestsClients requestsClients = new RequestsClients();
    private RequestOperations requestOperations = new RequestOperations();
    private AuthenticationToken authenticationToken = new AuthenticationToken();
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    [Theory]
    [InlineData(
        "CardOrder",
        "Дебетовая карта",
        "Mastercard",
        "Ткаченко",
        "Дмитрий",
        "Андреевич",
        "1998-12-06"
    )]
    [InlineData(
        "CardOrder",
        "Кредитная карта",
        "Visa",
        "Ткаченко",
        "Дмитрий",
        "Андреевич",
        "1998-12-06"
    )]
    // можно написать все 8 тестов, но особого смысла в них нет, так как от них фукнционал никак не зависит и не меняется

    public async Task OprderingACard(
        string operationCode,
        string cardType,
        string releaseProgram,
        string lastName,
        string firstName,
        string middleName,
        string birthdate
    )
    {
        var body = new List<OperationInfo>()
        {
            new OperationInfo { Identifier = "Product", Value = cardType },
            new OperationInfo { Identifier = "ProgramType", Value = releaseProgram },
            new OperationInfo { Identifier = "LastName", Value = lastName },
            new OperationInfo { Identifier = "FirstName", Value = firstName },
            new OperationInfo { Identifier = "MiddleName", Value = middleName },
            new OperationInfo { Identifier = "Birthdate", Value = birthdate },
        };

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
        Assert.NotNull(accessToken);
        var (putRequest, putData) =
            await requestOperations.UniversalPutRequestForEndpointOperations(
                operationCode,
                accessToken
            );

        var (patchRequest, patchData) =
            await requestOperations.UniversalPatchRequestForEndpointOperations(body, accessToken);
        var (postRequest, postData) =
            await requestOperations.UniversalPostRequestForEndpointOperations(
                accessToken,
                putData.RequestId
            );
        // Assert
        // put
        Console.WriteLine($"[ORDERING_A_CARD_PUT_REQUEST] Status Code:{putRequest.StatusCode}");
        Console.WriteLine($"[ORDERING_A_CARD_PUT_REQUEST] RequestId:{putData.RequestId}");
        Console.WriteLine($"[ORDERING_A_CARD_PUT_REQUEST] IsConfirmed:{putData.IsConfirmed}");
        Console.WriteLine($"[ORDERING_A_CARD_PUT_REQUEST] IsFinished:{putData.IsFinished}");
        Console.WriteLine($"[ORDERING_A_CARD_PUT_REQUEST] IsFinished:{putRequest.Content}");
        // patch
        Console.WriteLine($"[ORDERING_A_CARD_PATCH_REQUEST] Status Code:{patchRequest.StatusCode}");
        Console.WriteLine($"[ORDERING_A_CARD_PATCH_REQUEST] RequestId:{patchData.RequestId}");
        Console.WriteLine($"[ORDERING_A_CARD_PATCH_REQUEST] IsConfirmed:{patchData.IsConfirmed}");
        Console.WriteLine($"[ORDERING_A_CARD_PATCH_REQUEST] IsFinished:{patchData.IsFinished}");
        // post
        Console.WriteLine($"[ORDERING_A_CARD_POST_REQUEST] Status Code:{postRequest.StatusCode}");
        Console.WriteLine($"[ORDERING_A_CARD_POST_REQUEST] RequestId:{postData.RequestId}");
        Console.WriteLine($"[ORDERING_A_CARD_POST_REQUEST] IsConfirmed:{postData.IsConfirmed}");
        Console.WriteLine($"[ORDERING_A_CARD_POST_REQUEST] IsFinished:{postData.IsFinished}");
        //
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
        // await requestsClients.RequestForEndpointCreateClients();
        // await requestsClients.PostAuthentificationToken();

        // // PUT
        // var putRequest = await requestOperations.UniversalPutRequestForEndpointOperations(operationCode);
        // Console.WriteLine($"PutOrderingACardContent: {putRequest.Content}");
        // Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        // Assert.NotEqual(0,requestOperations.putOperationInfo.RequestId);
        // Assert.Equal(HttpStatusCode.OK, putRequest.StatusCode);
        // Assert.False(requestOperations.putOperationInfo.IsConfirmed);
        // Assert.False(requestOperations.putOperationInfo.IsFinished);

        // // PATCH
        // var patchRequest = await requestOperations.UniversalPatchRequestForEndpointOperations(body);
        // Console.WriteLine($"PatchOrderingACardStatus: {patchRequest.StatusCode}");
        // Console.WriteLine($"PatchOrderingACardContent: {patchRequest.Content}");
        // Console.WriteLine($"PatchOrderingACardStatus: {patchRequest.StatusCode}");
        // Assert.Equal(HttpStatusCode.OK, patchRequest.StatusCode);
        // Assert.False(requestOperations.patchOperationInfo.IsConfirmed);
        // Assert.True(requestOperations.patchOperationInfo.IsFinished);

        // // POST
        // var postRequest = await requestOperations.UniversalPostRequestForEndpointOperations();
        // Console.WriteLine($"PostOrderingACardContent: {postRequest.Content}");
        // Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        // Assert.NotEqual(0,requestOperations.postOperationInfo.RequestId);
        // Assert.Equal(HttpStatusCode.OK, postRequest.StatusCode);
        // Assert.True(requestOperations.postOperationInfo.IsConfirmed);
        // Assert.True(requestOperations.postOperationInfo.IsFinished);
    }
}
