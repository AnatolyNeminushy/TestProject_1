namespace Operations;

/// <summary>
/// Модель данных для хранения информации о счете пользователя
/// </summary>
public class BankAccount
{
    public int Id { get; set; }
    public string? Number { get; set; }
    public decimal Balance { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
}
