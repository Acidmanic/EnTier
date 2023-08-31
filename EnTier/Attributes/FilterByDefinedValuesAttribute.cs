using System.Collections.Generic;

namespace EnTier.Attributes;

public class FilterByDefinedValuesAttribute:FilterByCollectionValuesAttribute
{
    public FilterByDefinedValuesAttribute(params string[] definedValues)
    {
        ValuesCollection.AddRange(definedValues);
    }
    
    
}