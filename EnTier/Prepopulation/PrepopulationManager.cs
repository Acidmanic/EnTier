using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.Prepopulation.Extensions;
using EnTier.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnTier.Prepopulation
{
    public class PrepopulationManager
    {
        private static PrepopulationManager _instance = null;
        private static readonly object Locker = new object();

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


        public void PerformPrepopulation(IServiceResolver resolver, List<Type> seedTypes)
        {
            var logger = resolver.Resolve(typeof(ILogger)) as ILogger ?? NullLogger.Instance;

            var dependencyMap = GetDependencyMap(seedTypes, logger);

            var orderedSeedTypes = new DependencyResolver<Type>().OrderByDependency(dependencyMap);

            foreach (var seedType in orderedSeedTypes)
            {
                try
                {
                    var seed = (IPrepopulationSeed)resolver.Resolve(seedType);

                    if (seed != null)
                    {
                        logger.LogInformation("Running {SeedName}", seedType.Name);

                        var result = seed.Seed();

                        if (result)
                        {
                            logger.LogInformation("{SeedName} has been run Successfully.", seedType.Name);    
                        }
                        else
                        {
                            logger.LogError("{SeedName} has been run With failure.", seedType.Name);
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not create {SeedTypeFullName} because of an error: {Exception}",
                        seedType.FullName, e);
                    logger.LogDebug(
                        "It is probable, that you have missed registering your seed type {seedTypeName}," +
                        " or one of its dependencies on your DI.", seedType.Name);
                }
            }
        }


        public void PerformPrepopulation(IServiceResolver resolver, params Assembly[] assemblies)
        {
            var seedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetAvailableTypes()
                    .Where(PrepopulationSeedExtensions.IsASeedType);

                seedTypes.AddRange(types);
            }

            PerformPrepopulation(resolver, seedTypes);
        }


        private Dictionary<Type, List<Type>> GetDependencyMap(List<Type> seedTypes, ILogger logger)
        {
            var map = new Dictionary<Type, List<Type>>();

            var searchList = new List<Type>(seedTypes);
            var allDetectedSeeds = new List<Type>(seedTypes);

            while (searchList.Count > 0)
            {
                var missedDependencies = new List<Type>();

                foreach (var seedType in seedTypes)
                {
                    if (!map.ContainsKey(seedType))
                    {
                        var dependencies = new List<Type>();

                        var referencedSeeds = seedType.GetMarkedDependencies();

                        foreach (var referencedSeed in referencedSeeds)
                        {
                            if (referencedSeed.IsASeedType() && !dependencies.Contains(referencedSeed))
                            {
                                dependencies.Add(referencedSeed);

                                if (!allDetectedSeeds.Contains(referencedSeed))
                                {
                                    logger.LogWarning("Seed {MissedSeed} was not mentioned to be seeded but " +
                                                      "{Referencer} depends on it so it has been added to seeds.",
                                        referencedSeed.Name, seedType.Name);

                                    missedDependencies.Add(referencedSeed);
                                }
                            }
                        }

                        map.Add(seedType, dependencies);

                        searchList.Clear();
                        searchList.AddRange(missedDependencies);
                        allDetectedSeeds.AddRange(missedDependencies);
                    }
                }
            }

            return map;
        }
    }
}