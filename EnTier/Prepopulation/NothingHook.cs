using System.Diagnostics;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;

namespace EnTier.Prepopulation;

public class NothingHook<TStorage> : ISeedingHook<TStorage>
{
    public void PreInsertion(TStorage model, SeedingToolBox toolBox)
    {
    }

    public void PostInsertion(TStorage model, SeedingToolBox toolBox)
    {
    }

    public void PreIndexing(TStorage model, SeedingToolBox toolBox)
    {
    }

    public void PostIndexing(TStorage model, SeedingToolBox toolBox)
    {
    }
}