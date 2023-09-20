using System;
using EnTier.Prepopulation.Interfaces;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PrepopulationLevelAttribute:Attribute,IPrepopulationLevel
{
    
    public PrepopulationLevels Level { get;  }

    public Type DomainType { get; }
    
    public Type DomainIdType { get; }
    
    public PrepopulationLevelAttribute()
    {
        Level = PrepopulationLevels.Storage;
        DomainType = typeof(object); // instead of null
        DomainIdType = typeof(long);//instead of null
    }

    public PrepopulationLevelAttribute(Type domainType, Type domainIdType)
    {
        Level = PrepopulationLevels.Service;
        DomainType = domainType;
        DomainIdType = domainIdType;
    }
    
}