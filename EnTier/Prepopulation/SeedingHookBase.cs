using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation;

public abstract class SeedingHookBase<TStorage> : ISeedingHook<TStorage>
{
    public virtual void PreInsertion(TStorage model, SeedingToolBox toolBox)
    {
    }

    public virtual void PostInsertion(TStorage model, SeedingToolBox toolBox)
    {
    }

    public virtual void PreIndexing(TStorage model, SeedingToolBox toolBox)
    {
    }

    public virtual void PostIndexing(TStorage model, SeedingToolBox toolBox)
    {
    }
}