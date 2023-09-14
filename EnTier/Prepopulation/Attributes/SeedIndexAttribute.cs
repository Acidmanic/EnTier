using System;

namespace EnTier.Prepopulation.Attributes;

public class SeedIndexAttribute:Attribute
{
    public SeedIndexAttribute():this(false)
    {
    }
    
    public SeedIndexAttribute(bool fullTreeIndexing)
    {
        FullTreeIndexing = fullTreeIndexing;
    }

    public bool FullTreeIndexing { get; }
    
    
}