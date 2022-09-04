using System.Collections.Generic;
using EnTier.Results;

namespace EnTier.Prepopulation

{
    public abstract class PrepopulationSeedBase : IPrepopulationSeed
    {
        public abstract Result Seed();

        public abstract void Clear();

        public List<string> DependencySeedTags { get; } = new List<string>();

        public string Tag => this.GetType().FullName;

        public static string GetTag<T>()
            where T : IPrepopulationSeed
        {
            return typeof(T).FullName;
        }

        protected void DependOn<T>()
        where T:IPrepopulationSeed
        {
            var dependencyTag = GetTag<T>();
            
            if (!DependencySeedTags.Contains(dependencyTag))
            {
                DependencySeedTags.Add(dependencyTag);
            }
        }
    }
}