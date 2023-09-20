using System.Collections.Generic;
using Acidmanic.Utilities.Results;
using EnTier.Prepopulation;
using EnTier.Prepopulation.Attributes;
using EnTier.Prepopulation.Contracts;
using Example.Prepopulation.Models;
using Example.Prepopulation.Prepopulation.Hooks;

namespace Example.Prepopulation.Prepopulation;

[PrepopulationLevel(typeof(Chocolate),typeof(long))]
public class ChocolatesSeed:SeedBase<Chocolate>
{

    public static readonly Chocolate[] KnowsChocolates =
    {
        new Chocolate
        {
            FactoryName = "KitKat Factory!",
            ProductName = "KitKat"
        },
        new Chocolate
        {
            FactoryName = "Sneakers Factory!",
            ProductName = "Sneakers"
        }
    };


    public override IEnumerable<Chocolate> SeedingObjects => KnowsChocolates;

    public override Result<ISeedingHook<Chocolate>> HooksIntoSeedingBehavior => 
        new Result<ISeedingHook<Chocolate>>(true,new ChocolateLoggerHook());
}