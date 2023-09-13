using System.Collections.Generic;
using Acidmanic.Utilities.Results;

namespace EnTier.Prepopulation.Contracts;


public interface ISeed<TStorage>
{
    
    
    /// <summary>
    /// This is used for being recognized in debugging messages.
    /// </summary>
    string SeedName { get; }
    
    IEnumerable<TStorage> SeedingObjects { get; }

    
    Result<ISeedingHook<TStorage>> HooksIntoSeedingBehavior { get; } 

}