using RestSharp;

namespace TestProjectIntern_n1.RestClients;

public class AccountsRestClient : BaseRestClient
{
    public async Task<RestResponse> GetAccount(string accessToken)
    {
        var request = CreateBaseRequest("api/accounts", Method.Get);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        return await Client.ExecuteAsync(request);
    }
}

