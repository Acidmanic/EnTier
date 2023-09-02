using System;

namespace EnTier.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FullTreeReadAttribute:Attribute
{
    public FullTreeReadAttribute(bool readById,bool readAll)
    {
        ReadAll = readAll;
        ReadById = readById;
    }

    public FullTreeReadAttribute(bool readById) : this(readById, false)
    {
        
    }

    public FullTreeReadAttribute():this(true,true)
    {
        
    }

    public bool ReadById { get; }
    
    public bool ReadAll { get; }
    
    
}