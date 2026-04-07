// Есть проблема битых символов, пока не знаю как решить, 
// как я понял проблема в Рестшарпе, так как в постмане объект не битый,
// но это просто предположение 
// * проблема с кодировкой решена, вроде работает корректно


using System.Text.Json;
using System.Net;
using Clients;

namespace Operations;
//
public class OpeningAnAccountTests
{
    private RequestsClients requestsClients = new RequestsClients();
    private RequestOperations requestOperations = new RequestOperations();
    private OperationInfo operationInfo = new OperationInfo();
   
    [Theory]
    [InlineData("AccountOpen","Текущий счёт","Российский Рубль","lastname639111196098143429","name639111196098143429","middlenamfffffe639111196098143429","1971-12-18")]
    [InlineData("AccountOpen","Накопительный счёт","Доллар США","lastname639111196098143429","name639111196098143429","middlenamfffffe639111196098143429","1971-12-18")]
    // можно написать все 8 тестов, но особого смысла в них нет, так как от них фукнционал никак не зависит и не меняется
    
    public async Task OpeningAnAccount(string operationCode, string accountType, string currency, string lastName,string firstName, string middleName, string birthdate)
    {
        var body = new List<OperationInfo>()
        {
             new OperationInfo
        {
            Identifier = "AccountType",
            Value = accountType
        },
        new OperationInfo
        {   
            Identifier = "Currency",
            Value = currency
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
        Console.WriteLine($"OpeningPutAccountContent: {putRequest.Content}");
        Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        Assert.NotEqual(0,requestOperations.putOperationInfo.RequestId);
        Assert.Equal(HttpStatusCode.OK, putRequest.StatusCode);
        Assert.False(requestOperations.putOperationInfo.IsConfirmed);
        Assert.False(requestOperations.putOperationInfo.IsFinished);

        // PATCH
        var patchRequest = await requestOperations.UniversalPatchRequestForEndpointOperations(body);
        Console.WriteLine($"OpeningPatchAccountStatus: {patchRequest.StatusCode}");
        Console.WriteLine($"OpeningPatchAccountContent: {patchRequest.Content}");
        Console.WriteLine($"OpeningPatchAccountStatus: {patchRequest.StatusCode}");
        Assert.Equal(HttpStatusCode.OK, patchRequest.StatusCode);
        Assert.False(requestOperations.patchOperationInfo.IsConfirmed);
        Assert.True(requestOperations.patchOperationInfo.IsFinished);

        // POST
        var postRequest = await requestOperations.UniversalPostRequestForEndpointOperations();
        Console.WriteLine($"OpeningPostAccountContent: {postRequest.Content}");
        Console.WriteLine($"requestId {requestOperations.putOperationInfo.RequestId}");
        Assert.NotEqual(0,requestOperations.postOperationInfo.RequestId);
        Assert.Equal(HttpStatusCode.OK, postRequest.StatusCode);
        Assert.True(requestOperations.postOperationInfo.IsConfirmed);
        Assert.True(requestOperations.postOperationInfo.IsFinished);
    }
}