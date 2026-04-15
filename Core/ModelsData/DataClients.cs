namespace TestProjectIntern_n1.Core.ModelsData;

/// <summary>
/// Модель данных для хранения информации о пользователе.
/// </summary>
public class DataClients
{
    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Отчество пользователя.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Пол пользователя.
    /// </summary>
    public string? Sex { get; set; }

    /// <summary>
    /// Адрес пользователя.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Дата рождения пользователя.
    /// </summary>
    public string? Birthdate { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Токен аутентификации.
    /// </summary>
    public string? AccessToken { get; set; }
}
