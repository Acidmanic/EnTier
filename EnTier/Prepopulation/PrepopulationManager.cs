using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Results;
using EnTier.Contracts;
using EnTier.Prepopulation.Attributes;
using EnTier.Prepopulation.Contracts;
using EnTier.Prepopulation.Extensions;
using EnTier.Prepopulation.Interfaces;
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
            var essence = resolver.Resolve(typeof(EnTierEssence)) as EnTierEssence;

            if (essence == null)
            {
                new ConsoleLogger().LogError("You forgot to introduce your di to EnTier");

                return new Result<SeedingToolBox>().FailAndDefaultValue();
            }

            return new Result<SeedingToolBox>(true,
                new SeedingToolBox(essence));
        }


        private object CreateProfile(Type seedType, IServiceResolver resolver)
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

        private IPrepopulationLevel GetLevel(Type type)
        {
            var att = 
                type.GetCustomAttribute<PrepopulationLevelAttribute>() ?? new PrepopulationLevelAttribute();

            return att;
        }

        private ISeedingPerformer CreateSeedPerformer(Type seedType, object profile, SeedingToolBox toolBox)
        {

            var level = GetLevel(seedType);

            if (level.Level == PrepopulationLevels.Service)
            {

                return ServiceSeedPerformer(seedType, profile, toolBox,level.DomainType,level.DomainIdType);
            }

            return RepositorySeedPerformer(seedType, profile, toolBox);
        }

        private ISeedingPerformer RepositorySeedPerformer(Type seedType, object profile, SeedingToolBox toolBox)
        {
            var genericArguments = seedType.GetSeedGenericArguments();

            var storageType = genericArguments[0];
            var idType = storageType.GetIdType();

            var seedPerformerGenericType = typeof(RepositoryLevelSeedPerformer<,>);
            var seedPerformerSpecificType = seedPerformerGenericType.MakeGenericType(storageType, idType);
            var profileType = profile.GetType();

            var constructor =
                seedPerformerSpecificType.GetConstructor(new Type[] { profileType, typeof(SeedingToolBox) });

            var performerIndex = constructor!.Invoke(new object[] { profile, toolBox });

            return performerIndex as ISeedingPerformer;
        }
        
        private ISeedingPerformer ServiceSeedPerformer(Type seedType, 
            object profile, 
            SeedingToolBox toolBox,
            Type domainType,
            Type domainIdType)
        {
            var genericArguments = seedType.GetSeedGenericArguments();

            var storageType = genericArguments[0];
            var storageIdType = storageType.GetIdType();

            var seedPerformerGenericType = typeof(ServiceLevelSeedPerformer<,,,>);
            var seedPerformerSpecificType = seedPerformerGenericType
                .MakeGenericType(domainType,storageType,domainIdType, storageIdType);
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
                var profile = CreateProfile(seedType, resolver);

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