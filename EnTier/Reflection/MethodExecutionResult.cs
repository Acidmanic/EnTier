using System;

namespace EnTier.Reflection;

public class MethodExecutionResult
{
    public bool Successful { get; set; }
    
    public Exception Exception { get; set; }
    
    public object ReturnValue { get; set; }
    
    public bool ReturnsValue { get; set; }
    
    public Type ReturnType { get; set; }
    
}