using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using EnTier.Contracts;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Extensions;
using EnTier.Prepopulation.Models;
using EnTier.Services.Transliteration;
using EnTier.UnitOfWork;
using EnTier.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

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


        private List<Type> ExtractAllSeedTypes(Assembly[] assemblies)
        {
            var seedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetAvailableTypes()
                    .Where(PrepopulationSeedExtensions.IsASeedType);

                seedTypes.AddRange(types);
            }

            return seedTypes;
        }


        private Result<SeedingToolBox> CreateToolBox(IServiceResolver resolver)
        {
            var logger = resolver.Resolve(typeof(ILogger)) as ILogger ?? new ConsoleLogger();

            var unitOfWork = resolver.Resolve(typeof(IUnitOfWork)) as IUnitOfWork;

            if (unitOfWork == null)
            {
                logger.LogError("Unable to get UnitOfWork. You might have forgotten to select your unit of work." +
                                "Prepopulation would be aborted.");

                return new Result<SeedingToolBox>().FailAndDefaultValue();
            }

            var transliterationService = resolver.Resolve(typeof(ITransliterationService)) as ITransliterationService
                                         ?? new EnTierBuiltinTransliterationsService();

            return new Result<SeedingToolBox>(true,
                new SeedingToolBox(unitOfWork, logger, transliterationService));
        }


        private object CreateProfile(Type seedType,IServiceResolver resolver)
        {
            var seedObject = seedType.NewOrResolve(resolver);
            
            var genericArguments = seedType.GetSeedGenericArguments();

            var storageType = genericArguments[0];

            var genericProfileType = typeof(SeedingProfile<>);
            var specificProfileType = genericProfileType.MakeGenericType(storageType);

            var profile = new ObjectInstantiator().BlindInstantiate(specificProfileType) as ISeedingProfile;

            profile!.LoadSeed(seedObject);
            
            return profile;
        }

        private ISeedingPerformer CreateSeedPerformer(Type seedType, object profile, SeedingToolBox toolBox)
        {
            var genericArguments = seedType.GetSeedGenericArguments();

            var storageType = genericArguments[0];
            var idType = storageType.GetIdType();

            var seedPerformerGenericType = typeof(SeedPerformer<,>);
            var seedPerformerSpecificType = seedPerformerGenericType.MakeGenericType(storageType, idType);
            var profileType = profile.GetType();

            var constructor =
                seedPerformerSpecificType.GetConstructor(new Type[] { profileType, typeof(SeedingToolBox) });

            var performerIndex = constructor!.Invoke(new object[] { profile, toolBox });

            return performerIndex as ISeedingPerformer;
        }

        public void PerformPrepopulation(IServiceResolver resolver, params Assembly[] assemblies)
        {
            var seedTypes = ExtractAllSeedTypes(assemblies);

            var toolBox = CreateToolBox(resolver);

            if (!toolBox)
            {
                return;
            }

            var dependencyMap = GetDependencyMap(seedTypes, toolBox.Value.Logger);

            var orderedSeedTypes = new DependencyResolver<Type>().OrderByDependency(dependencyMap);

            var seedPerformers = orderedSeedTypes.Select(seedType =>
            {
                var profile = CreateProfile(seedType,resolver);

                var performer = CreateSeedPerformer(seedType, profile, toolBox.Value);

                return performer;
            }).ToList();

            toolBox.Value.Logger.LogInformation("+++ Prepopulation.Seeding +++");

            foreach (var performer in seedPerformers)
            {
                performer.Seed();
            }

            toolBox.Value.Logger.LogInformation("+++ Prepopulation.Indexing +++");

            foreach (var performer in seedPerformers)
            {
                performer.Index();
            }
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