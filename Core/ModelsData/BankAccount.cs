namespace TestProjectIntern_n1.Core.ModelsData;

/// <summary>
/// Модель данных для хранения информации о счете пользователя.
/// </summary>
public class BankAccount
{
    /// <summary>
    /// Id счета.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер счета.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Баланс счета.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Название счета.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Состояние счета.
    /// </summary>
    public string? State { get; set; }
}
