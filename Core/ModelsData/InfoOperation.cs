namespace TestProjectIntern_n1.Core.ModelsData;

/// <summary>
/// Модель данных для хранения информации об операциях.
/// </summary>
public class InfoOperation
{
    /// <summary>
    /// Id запроса.
    /// </summary>
    public int RequestId { get; set; }

    /// <summary>
    /// Название операции.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Флаг завершения операции.
    /// </summary>
    public bool IsConfirmed { get; set; }

    /// <summary>
    /// Флаг подтверждения операции.
    /// </summary>
    public bool IsFinished { get; set; }
}
