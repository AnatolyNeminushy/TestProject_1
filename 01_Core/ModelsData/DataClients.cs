namespace BaseSpaceRequests;

/// <summary>
/// Модель данных для хранения информации о пользователе
/// </summary>
public class DataClients
{
    public string? PhoneNumber { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Sex { get; set; }
    public string? Address { get; set; }
    public string? Birthdate { get; set; }
    public string? Password { get; set; }

    // Токен аунтетификации
    public string? AccessToken { get; set; }
}
