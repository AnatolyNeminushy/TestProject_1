using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Operations;

public class OperationInfo{
    public int RequestId{get;set;}
    public string? Name{get;set;}
    public bool IsConfirmed{get;set;}
    public bool IsFinished{get;set;}
    public string? Identifier { get; set; }
    public string? Value { get; set; }
}