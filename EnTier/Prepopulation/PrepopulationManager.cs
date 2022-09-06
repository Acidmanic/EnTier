using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Logging;
using Microsoft.Extensions.Logging;

namespace EnTier.Prepopulation
{
    public class PrepopulationManager
    {
        private static PrepopulationManager _instance = null;
        private static readonly object Locker = new object();
        private static ILogger _logger = EnTierLogging.GetInstance().Logger;

        private PrepopulationManager()
        {
        }

        public static PrepopulationManager GetInstance()
        {
            lock (Locker)
            {
                if (_instance == null)
                {
                    _instance = new PrepopulationManager();
                }
            }

            return _instance;
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
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
                    _logger.LogError(e, "Could not create {SeedTypeFullName} because of an error.", seedType.FullName);
                    _logger.LogDebug(
                        "It is probable, that you have missed registering your seed type {seedType.Name}," +
                        " or one of its dependencies on your DI.", seedType.Name);
                }


                if (seed != null)
                {
                    seeds.Add(seed);
                }
            }

            SortByDependencies(seeds);

            foreach (var seed in seeds)
            {
                _logger.LogDebug("Running {seed.Tag}", seed.Tag);

                var result = seed.Seed();

                var msgResult = (result.Success ? "Successfully" : "With failure.");

                _logger.LogDebug("{SeedTag} has been run {MsgResult}", seed.Tag, msgResult);
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