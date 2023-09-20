using System.Collections.Generic;
using Acidmanic.Utilities.Results;

namespace EnTier.Prepopulation.Contracts;


public interface ISeed<TSeedModel>
{
    
    
    /// <summary>
    /// This is used for being recognized in debugging messages.
    /// </summary>
    string SeedName { get; }
    
    IEnumerable<TSeedModel> SeedingObjects { get; }

    
    Result<ISeedingHook<TSeedModel>> HooksIntoSeedingBehavior { get; }

    void Initialize();

}