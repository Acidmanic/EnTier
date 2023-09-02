using System;

namespace EnTier.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class FullTreeActionAttribute:Attribute
{
    public FullTreeActionAttribute(bool readById,bool readAll)
    {
        ReadAll = readAll;
        ReadById = readById;
    }

    public FullTreeActionAttribute(bool readById) : this(readById, false)
    {
        
    }

    public FullTreeActionAttribute():this(true,true)
    {
        
    }

    public bool ReadById { get; }
    
    public bool ReadAll { get; }
}