using TestProjectIntern_n1.Core.ModelsData;
using TestProjectIntern_n1.Core.Tools;
using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Базовые тестовые данные.
/// </summary>
public abstract class BaseTest
{
    /// <summary>
    /// Логин первого пользователя.
    /// </summary>
    protected const string Login = "DimaFire";

    /// <summary>
    /// Пароль первого пользователя.
    /// </summary>
    protected const string Password = "Dima5678";

    /// <summary>
    /// Логин второго пользователя.
    /// </summary>
    protected const string LoginAnotherAccount = "IvanWater";

    /// <summary>
    /// Пароль второго пользователя.
    /// </summary>
    protected const string PasswordAnotherAccount = "Ivan1234";

    /// <summary>
    /// Логин пользователя с банковскими счетами для проверки блокировки.
    /// </summary>
    protected const string LoginForLockAccount = "RomaIce";

    /// <summary>
    /// Пароль пользователя с банковскими счетами для проверки блокировки.
    /// </summary>
    protected const string PasswordForLockAccount = "RomaForever5643";

    /// <summary>
    /// Разница в сумме.
    /// </summary>
    protected const decimal DifferenceAmount = 10.00m;

    /// <summary>
    /// Клиент для создания пользователя.
    /// </summary>
    protected ClientsRestClient ClientsRestClient = new ClientsRestClient();

    /// <summary>
    /// Клиент для совершения операций.
    /// </summary>
    protected OperationsRestClient OperationsRestClient = new OperationsRestClient();

    /// <summary>
    /// Клиент для получения счета пользователя.
    /// </summary>
    protected AccountsRestClient AccountsRestClient = new AccountsRestClient();

    /// <summary>
    /// Клиент для получения карт пользователя.
    /// </summary>
    protected CardsRestClient CardsRestClient = new CardsRestClient();

    /// <summary>
    /// Клиент для получения токена аутентификаци.
    /// </summary>
    protected AuthenticationRestClient AuthenticationRestClient = new AuthenticationRestClient();

    /// <summary>
    /// Клиент для получения доступных продуктов.
    /// </summary>
    protected ShowcaseRestClient ShowcaseRestClient = new ShowcaseRestClient();

    /// <summary>
    /// Создание и блокировка банковского счета пользователя.
    /// </summary>
    protected async Task<BankAccount> CreateAndLockAccount()
    {
        var bodyCreateAccount = new List<ParametrOperation>()
        {
            new ParametrOperation { Identifier = "AccountType", Value = "Текущий счёт" },
            new ParametrOperation { Identifier = "Currency", Value = "Российский Рубль" }
        };
        var authenticationResponse = await AuthenticationRestClient.GetAuthenticationToken(LoginForLockAccount, PasswordForLockAccount);
        var authenticationData = JsonDeserializer.DeserializeData<DataClients>(authenticationResponse.Content);
        var accessToken = authenticationData.AccessToken;

        var startOperationResponse = await OperationsRestClient.StartOperation("AccountOpen", accessToken);
        var startOperationData = JsonDeserializer.DeserializeData<InfoOperation>(startOperationResponse.Content);

        var nextStepOperationResponse = await OperationsRestClient.NextStepOperation(startOperationData.RequestId, bodyCreateAccount, accessToken);
        var confirmedOperationResponse = await OperationsRestClient.ConfirmedOperation(startOperationData.RequestId, accessToken);

        var getAccountsResponse = await AccountsRestClient.GetAccounts(accessToken);
        var getAccountsData = JsonDeserializer.DeserializeData<List<BankAccount>>(getAccountsResponse.Content!);

        var account = getAccountsData
            .FirstOrDefault(x => x.State == "Active");

        var lockAccountResponse = await AccountsRestClient.LockAccount(account.Id, accessToken);
        var lockAccountData = JsonDeserializer.DeserializeData<BankAccount>(lockAccountResponse.Content);

        return lockAccountData;
    }
}

