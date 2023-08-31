using System;
using System.Collections.Generic;

namespace EnTier.Attributes;

public abstract class FilterByCollectionValuesAttribute:Attribute
{
    public List<string> ValuesCollection { get; private set; } = new List<string>();
}