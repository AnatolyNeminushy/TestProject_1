namespace Operations;

/// <summary>
/// Модель данных для хранения информации об операциях
/// </summary>
public class InfoOperation
{
    public int RequestId { get; set; }
    public string? Name { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsFinished { get; set; }
    public List<StepParams>? StepParams { get; set; }
}

/// <summary>
/// Модель данных StepParams
/// </summary>
public class StepParams
{
    public List<string>? Values { get; set; }
    public string? Identifier { get; set; }
}
