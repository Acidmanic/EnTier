using System;

namespace EnTier.EventSourcing.Attributes;

public class ApiResponseAttribute : Attribute
{
    public ApiResponseAttribute(bool aggregateRoot, bool methodResult)
    {
        AggregateRoot = aggregateRoot;
        MethodResult = methodResult;
    }

    public bool AggregateRoot { get; }

    public bool MethodResult { get; }
}