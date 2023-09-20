using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Models;
using Example.Prepopulation.Models;
using Microsoft.Extensions.Logging;

namespace Example.Prepopulation.Prepopulation.Hooks;


public class ChocolateLoggerHook:ISeedingHook<Chocolate>
{
    public void PreInsertion(Chocolate model, SeedingToolBox toolBox)
    {
        toolBox.Logger.LogInformation("HOOK: Pre-Insertion of {Name}",model.ProductName );
    }

    public void PostInsertion(Chocolate model, SeedingToolBox toolBox)
    {
        toolBox.Logger.LogInformation("HOOK: Post-Insertion of {Name}",model.ProductName );
    }

    public void PreIndexing(Chocolate model, SeedingToolBox toolBox)
    {
        toolBox.Logger.LogInformation("HOOK: Post-Indexing of {Name}",model.ProductName );
    }

    public void PostIndexing(Chocolate model, SeedingToolBox toolBox)
    {
        toolBox.Logger.LogInformation("HOOK: Post-Indexing of {Name}",model.ProductName );
    }
}