


using System.Text.Json;
using System.Net;
using Clients;

namespace Operations;
//
public class OrderingACardTests
{
    private RequestsClients requestsClients = new RequestsClients();
    private RequestOperations requestOperations = new RequestOperations();
    private OperationInfo operationInfo = new OperationInfo();
   
    [Theory]
    [InlineData("CardOrder","Дебетовая карта","Mastercard","lastname639111196098143429","name639111196098143429","middlenamfffffe639111196098143429","1971-12-18")]
    [InlineData("CardOrder","Кредитная карта","Visa","lastname639111196098143429","name639111196098143429","middlenamfffffe639111196098143429","1971-12-18")]
    // можно написать все 8 тестов, но особого смысла в них нет, так как от них фукнционал никак не зависит и не меняется
    
    public async Task OprderingACard(string operationCode, string cardType, string releaseProgram, string lastName,string firstName, string middleName, string birthdate)
    {
        var body = new List<OperationInfo>()
        {
             new OperationInfo
        {
            Identifier = "Product",
            Value = cardType
        },
        new OperationInfo
        {   
            Identifier = "ProgramType",
            Value = releaseProgram
        },
        new OperationInfo
        {   
            Identifier = "LastName",
            Value = lastName
        },
        new OperationInfo
        {   
            Identifier = "FirstName",
            Value = firstName
        },
        new OperationInfo
        {   
            Identifier = "MiddleName",
            Value = middleName
        },
        new OperationInfo
        {   
            Identifier = "Birthdate",
            Value = birthdate
        },
        };
        
        
        // Asserts
        // PUT
        var putRequest = await requestOperations.UniversalPutRequestForEndpointOperations(operationCode);
        Console.WriteLine($"PutOrderingACardContent: {putRequest.Content}");
        Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        Assert.NotEqual(0,requestOperations.putOperationInfo.RequestId);
        Assert.Equal(HttpStatusCode.OK, putRequest.StatusCode);
        Assert.False(requestOperations.putOperationInfo.IsConfirmed);
        Assert.False(requestOperations.putOperationInfo.IsFinished);

        // PATCH
        var patchRequest = await requestOperations.UniversalPatchRequestForEndpointOperations(body);
        Console.WriteLine($"PatchOrderingACardStatus: {patchRequest.StatusCode}");
        Console.WriteLine($"PatchOrderingACardContent: {patchRequest.Content}");
        Console.WriteLine($"PatchOrderingACardStatus: {patchRequest.StatusCode}");
        Assert.Equal(HttpStatusCode.OK, patchRequest.StatusCode);
        Assert.False(requestOperations.patchOperationInfo.IsConfirmed);
        Assert.True(requestOperations.patchOperationInfo.IsFinished);

        // POST
        var postRequest = await requestOperations.UniversalPostRequestForEndpointOperations();
        Console.WriteLine($"PostOrderingACardContent: {postRequest.Content}");
        Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        Assert.NotEqual(0,requestOperations.postOperationInfo.RequestId);
        Assert.Equal(HttpStatusCode.OK, postRequest.StatusCode);
        Assert.True(requestOperations.postOperationInfo.IsConfirmed);
        Assert.True(requestOperations.postOperationInfo.IsFinished);
    }
}