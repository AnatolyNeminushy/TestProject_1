using TestProjectIntern_n1.RestClients;

namespace TestProjectIntern_n1.Tests;

/// <summary>
/// Базовые тестовые данные.
/// </summary>
public abstract class BaseTests
{
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    protected const string Login = "DimaFire";

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    protected const string Password = "Dima5678";

    /// <summary>
    /// Логин другого пользователя.
    /// </summary>
    protected const string LoginAnotherAccount = "IvanWater";

    /// <summary>
    /// Пароль другого пользователя.
    /// </summary>
    protected const string PasswordAnotherAccount = "Ivan1234";

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
    /// Клиент для получения токена аутентификаци.
    /// </summary>
    protected AuthenticationRestClient AuthenticationRestClient = new AuthenticationRestClient();
}

