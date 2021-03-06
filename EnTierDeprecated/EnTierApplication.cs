

using EnTier.Configuration;
using EnTier.Binding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using EnTier.Utility;
using System;
using Microsoft.EntityFrameworkCore;
using EnTier.Service;
using EnTier.Repository;
using EnTier.Components;
using EnTier.Context;

namespace EnTier
{


    internal static class EnTierApplication
    {


        public static IDIResolver Resolver { get; private set; }

        public static IDIRegisterer Registerer { get; private set; }
        public static bool IsContextBased { get; private set; }

        public static void Initialize(IServiceCollection services)
        {
            Registerer = new MicrosoftDependencyInjectionRegisterer(services);

            PerformRegistrations();
        }

        public static void Initialize(IApplicationBuilder app)
        {
            Resolver = new MicrosoftDependencyInjectionResolver(app.ApplicationServices);

            ApplicationStart();
        }

        public static Type ContextType { get; set; } = typeof(NullContext);

        public static bool UseConfiguredContextType { get; set; } = false;


        private static void PerformRegistrations()
        {
            //TODO: Later figure a way to give the code user the chance to choos what 
            Registerer.RegisterSingleton<IProvider<EnTierConfigurations>, MSExtensionsConfigurationsProvider>();

            Registerer.RegisterTransient<IDatasetAccessor, InjectionDatasetAccessor>();

        }



        private static void ApplicationStart()
        {

            IsContextBased = ReflectionService.Make().IsAnyExtensionFor<DbContext>();

        }


    }
}