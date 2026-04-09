// Есть проблема битых символов, пока не знаю как решить,
// как я понял проблема в Рестшарпе, так как в постмане объект не битый,
// но это просто предположение
// * проблема с кодировкой решена, вроде работает корректно

using System.Net;
using System.Text.Json;
using BaseSpaceRequest;
using Clients;

namespace Operations;

//
public class OpeningAnAccountTests
{
    private RequestsClients requestsClients = new RequestsClients();
    private RequestOperations requestOperations = new RequestOperations();

    private AuthenticationToken authenticationToken = new AuthenticationToken();
    JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    // private OperationInfo operationInfo = new OperationInfo();

    [Theory]
    [InlineData(
        "AccountOpen",
        "Текущий счёт",
        "Российский Рубль",
        "Ткаченко",
        "Дмитрий",
        "Андреевич",
        "1998-12-06"
    )]
    [InlineData(
        "AccountOpen",
        "Накопительный счёт",
        "Доллар США",
        "Ткаченко",
        "Дмитрий",
        "Андреевич",
        "1998-12-06"
    )]
    // можно написать все 8 тестов, но особого смысла в них нет, так как от них фукнционал никак не зависит и не меняется

    public async Task OpeningAnAccount(
        string operationCode,
        string accountType,
        string currency,
        string lastName,
        string firstName,
        string middleName,
        string birthdate
    )
    {
        var body = new List<OperationInfo>()
        {
            new OperationInfo { Identifier = "AccountType", Value = accountType },
            new OperationInfo { Identifier = "Currency", Value = currency },
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
        // Asserts
        // put
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PUT_REQUEST] Status Code:{putRequest.StatusCode}");
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PUT_REQUEST] RequestId:{putData.RequestId}");
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PUT_REQUEST] IsConfirmed:{putData.IsConfirmed}");
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PUT_REQUEST] IsFinished:{putData.IsFinished}");
        // patch
        Console.WriteLine(
            $"[OPENING_AN_ACCOUNT_PATCH_REQUEST] Status Code:{patchRequest.StatusCode}"
        );
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PATCH_REQUEST] RequestId:{patchData.RequestId}");
        Console.WriteLine(
            $"[OPENING_AN_ACCOUNT_PATCH_REQUEST] IsConfirmed:{patchData.IsConfirmed}"
        );
        Console.WriteLine($"[OPENING_AN_ACCOUNT_PATCH_REQUEST] IsFinished:{patchData.IsFinished}");
        // post
        Console.WriteLine(
            $"[OPENING_AN_ACCOUNT_POST_REQUEST] Status Code:{postRequest.StatusCode}"
        );
        Console.WriteLine($"[OPENING_AN_ACCOUNT_POST_REQUEST] RequestId:{postData.RequestId}");
        Console.WriteLine($"[OPENING_AN_ACCOUNT_POST_REQUEST] IsConfirmed:{postData.IsConfirmed}");
        Console.WriteLine($"[OPENING_AN_ACCOUNT_POST_REQUEST] IsFinished:{postData.IsFinished}");
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
    }
}
