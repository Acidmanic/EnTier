

using EnTier.Configuration;
using EnTier.Binding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using EnTier.Utility;
using System;
using Microsoft.EntityFrameworkCore;
using EnTier.Components;
using EnTier.Context;
using System.Diagnostics;
using EnTier.Binding.Abstractions;

namespace EnTier
{


    internal static class EnTierApplication
    {


        public static IDIResolver Resolver { get; private set; }

        public static Type ContextType { get; private set; } = typeof(NullContext);

        public static bool UseConfiguredContextType { get; private set; } = false;

        //TODO: Add Following Property, initialize in startapp
        //public static bool IsEntityFrameworkPresent { get; private set; } = false;

        public static void Configure(IDIResolver resolver
                                   , bool useConfiguredContextType
                                   , Type contextType) 
        {
            Resolver = resolver;

            UseConfiguredContextType = useConfiguredContextType;

            ContextType = contextType;

            ApplicationStart();

        }


        public static void PerformRegistrations(IDIRegisterer registerer)
        {
            //TODO: Later figure a way to give the code user the chance to choos what 
            registerer.RegisterSingleton<IProvider<EnTierConfigurations>, MSExtensionsConfigurationsProvider>();

            registerer.RegisterTransient<IDatasetAccessor, InjectionDatasetAccessor>();

        }

        private static void ApplicationStart()
        {

            CacheReflectionAhead();

        }

        private static void CacheReflectionAhead()
        {
            ReflectionService.Make().CacheCurrent();

            var s = new StackTrace();

            var caller = s.GetFrame(1);

            var ass = caller.GetMethod().DeclaringType.Assembly;

            ReflectionService.Make().Cache(ass);
        }
    }
}