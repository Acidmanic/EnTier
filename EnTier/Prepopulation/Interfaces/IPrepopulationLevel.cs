using System;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation.Interfaces;

public interface IPrepopulationLevel
{
    public PrepopulationLevels Level { get;  }

    public Type DomainType { get; }
    
    public Type DomainIdType { get; }
}