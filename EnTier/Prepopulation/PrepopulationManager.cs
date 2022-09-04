using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;

namespace EnTier.Prepopulation
{
    public class PrepopulationManager
    {
        private static PrepopulationManager _instance = null;
        private static object _locker = new object();
        private Action<string> _log = Console.WriteLine;

        private PrepopulationManager()
        {
        }

        public static PrepopulationManager GetInstance()
        {
            lock (_locker)
            {
                if (_instance == null)
                {
                    _instance = new PrepopulationManager();
                }
            }

            return _instance;
        }

        public void SetLogger(Action<string> log)
        {
            _log = log;
        }


        public void PerformPrepopulation(IServiceResolver resolver, List<Type> seedTypes)
        {
            var seeds = new List<IPrepopulationSeed>();

            foreach (var seedType in seedTypes)
            {
                IPrepopulationSeed seed = null;

                try
                {
                    seed = (IPrepopulationSeed) resolver.Resolve(seedType);
                }
                catch (Exception e)
                {
                    _log($"Could not create {seedType.FullName} because of an error: {e.Message}");
                    _log($"It is probable, that you have missed registering your seed type {seedType.Name}," +
                         $" or one of its dependencies on your DI.");
                }


                if (seed != null)
                {
                    seeds.Add(seed);
                }
            }

            SortByDependencies(seeds);

            foreach (var seed in seeds)
            {
                _log("Running " + seed.Tag);

                var result = seed.Seed();

                _log($"{seed.Tag} has been run {(result.Success ? "Successfully" : "With failure.")}");
            }
        }

        public void PerformPrepopulation(IServiceResolver resolver, params Assembly[] assemblies)
        {
            bool ValidSeedSelector(Type t) =>
                (!t.IsAbstract && !t.IsInterface) && TypeCheck.Implements<IPrepopulationSeed>(t);

            var seedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetAvailableTypes()
                    .Where((Func<Type, bool>) ValidSeedSelector);
                
                seedTypes.AddRange(types);
            }
           
            PerformPrepopulation(resolver, seedTypes);
        }

        //TODO: check for infinite loop
        private void SortByDependencies(List<IPrepopulationSeed> seeds)
        {
            var dirty = true;

            while (dirty)
            {
                dirty = false;

                for (int i = 0; i < seeds.Count - 1; i++)
                {
                    var current = seeds[i];
                    var next = seeds[i + 1];

                    if (current.DependencySeedTags.Contains(next.Tag))
                    {
                        seeds[i] = next;
                        seeds[i + 1] = current;

                        dirty = true;
                    }
                }
            }
        }
    }
}