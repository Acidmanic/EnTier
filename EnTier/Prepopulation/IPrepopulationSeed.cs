using System.Collections.Generic;
using Acidmanic.Utilities.Results;

namespace EnTier.Prepopulation
{
    public interface IPrepopulationSeed
    {

        Result Seed();

        void Clear();
        
        List<string> DependencySeedTags { get; }
        
        string Tag { get; }
    }
}