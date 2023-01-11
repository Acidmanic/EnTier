using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using EnTier.Prepopulation.Attributes;

namespace EnTier.Prepopulation.Extensions
{
    public static class PrepopulationSeedExtensions
    {


        public static List<Type> GetMarkedDependencies(this IPrepopulationSeed seed)
        {
            if (seed != null)
            {
                var type = seed.GetType();

                return GetMarkedDependencies(type);
            }

            return new List<Type>();
        }

        public static List<Type> GetMarkedDependencies(this Type type)
        {
            if (IsASeedType(type))
            {
                var dependencies = new List<Type>();


                var markedTypes = type.GetCustomAttributes<DependsOnSeedAttribute>()
                    .Select(att => att.DependeeType);

                foreach (var markedType in markedTypes)
                {
                    if (markedType.IsASeedType())
                    {
                        dependencies.Add(markedType);
                    }
                }
                
                return dependencies;
            }
            return new List<Type>();
        }

        public static bool IsASeedType(this Type type)
        {
            return (!type.IsAbstract && !type.IsInterface) && TypeCheck.Implements<IPrepopulationSeed>(type);
        }
    }
}