using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Prepopulation.Attributes;
using EnTier.Prepopulation.Contracts;

namespace EnTier.Prepopulation.Models;

internal interface ISeedingProfile
{
    internal void LoadSeed(object seedObject);
}

internal class SeedingProfile<TStorage>:ISeedingProfile
{
    public IEnumerable<TStorage> SeedData { get; set; }

    public  bool AlsoIndex { get;  set;}
    
    public  bool FullTreeIndexing { get;  set;}
    
    public  string Name { get;  set;}
    
    public ISeedingHook<TStorage> Hook { get; set; }

    public Action Initialize { get; set; }
    

     void ISeedingProfile.LoadSeed(object seedObject)
    {
        if (seedObject is ISeed<TStorage> seed)
        {
            Hook = seed.HooksIntoSeedingBehavior ? seed.HooksIntoSeedingBehavior.Value : new NothingHook<TStorage>();
            Name = seed.SeedName;
            SeedData = seed.SeedingObjects;

            var att = seedObject.GetType().GetCustomAttributes<SeedIndexAttribute>()
                .FirstOrDefault();

            if (att != null)
            {
                AlsoIndex = true;
                FullTreeIndexing = att.FullTreeIndexing;
            }
            else
            {
                AlsoIndex = false;
                FullTreeIndexing = false;
            }

            Initialize = () => seed.Initialize();
        }
    }
}