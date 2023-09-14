using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using EnTier.Prepopulation.Attributes;
using EnTier.Prepopulation.Contracts;

namespace EnTier.Prepopulation.Extensions
{
    internal static class PrepopulationSeedExtensions
    {
        public static List<Type> GetMarkedDependencies<TStorage>(this ISeed<TStorage> seed)
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

        public static Type[] GetSeedGenericArguments(this Type type)
        {
            var interfaceType = type.GetSeedInterface();

            if (interfaceType != null)
            {
                return interfaceType.GetGenericArguments();
            }

            return new Type[] { };
        }

        public static Type GetSeedInterface(this Type type)
        {
            var interfaces = type.GetInterfaces();

            return interfaces.FirstOrDefault(i => TypeCheck.IsSpecificOf(i, typeof(ISeed<>)));
        }

        public static object New(this Type type)
        {
            return type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
        }

        internal static object NewOrResolve(this Type type, IServiceResolver resolver)
        {
            var instantiated = type.GetConstructor(new Type[] { })?.Invoke(new object[] { });

            if (instantiated != null)
            {
                return instantiated;
            }
            return resolver.Resolve(type);
        }

        public static bool IsASeedType(this Type type)
        {
            if (!type.IsAbstract && !type.IsInterface)
            {
                var interfaces = type.GetInterfaces();

                if (interfaces.Any(i => TypeCheck.IsSpecificOf(i, typeof(ISeed<>))))
                {
                    return true;
                }
            }

            return false;
        }

        public static Type GetIdType(this Type storageType)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf(storageType);

            if (idLeaf != null)
            {
                return idLeaf.Type;
            }

            return null;
        }
    }
}