using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation.Contracts;

public interface ISeedingHook<TStorage>
{
    
    void PreInsertion(TStorage model, SeedingToolBox toolBox);
    
    void PostInsertion(TStorage model, SeedingToolBox toolBox);

    void PreIndexing(TStorage model, SeedingToolBox toolBox);

    void PostIndexing(TStorage model, SeedingToolBox toolBox);
}