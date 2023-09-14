using System.Collections.Generic;
using Acidmanic.Utilities.Results;
using EnTier.Prepopulation.Contracts;

namespace EnTier.Prepopulation;

public class SeedBase<TStorage> : ISeed<TStorage>
{
    public virtual string SeedName => GetName();

    private string GetName()
    {
        var name = this.GetType().Name;

        var endings = new string[] { "seed", "seeds", "prepopulation" };

        foreach (var ending in endings)
        {
            if (name.ToLower().EndsWith(ending))
            {
                name = name.Substring(0, name.Length - ending.Length);
            }
        }

        return name;
    }

    public virtual IEnumerable<TStorage> SeedingObjects { get; } = new List<TStorage>();

    public virtual Result<ISeedingHook<TStorage>> HooksIntoSeedingBehavior =>
        new Result<ISeedingHook<TStorage>>().FailAndDefaultValue();

    public virtual void Initialize()
    {
    }
}