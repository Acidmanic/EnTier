using EnTier;
using EnTier.Binding;
using EnTier.Binding.Abstractions;
using EnTier.Configuration;
using EnTier.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnTier.Bootstrap
{
    internal static class Boot
    {
        private static IDIResolver _resolver;
        private static bool _useConfiguredContextType = false;
        private static Type _contextType = typeof(NullContext);


        private class EnTierApplicationConfigurer : IEnTierApplicationConfigurer
        {
            public IEnTierApplicationConfigurer SetContext<T>()
            {

                _contextType = typeof(T);

                _useConfiguredContextType = true;

                return this;
            }
        }

        public static void Strap(IServiceCollection services)
        {
            var registerer = new MicrosoftDependencyInjectionRegisterer(services);

            EnTierApplication.PerformRegistrations(registerer);
        }

        public static void Strap(IApplicationBuilder app)
        {
            _resolver = new MicrosoftDependencyInjectionResolver(app.ApplicationServices);

            //Entry For Core.Asp
            ConfigureEnTieerApplication();
        }

        public static void Strap(Action<IEnTierApplicationConfigurer> configurer)
        {
            if (configurer != null)
            {
                configurer(new EnTierApplicationConfigurer());
            }
        }

        public static void Strap(IDIResolver resolver)
        {
            _resolver = resolver;
            
        }

        public static void Strap(IDIRegisterer registerer)
        {
            EnTierApplication.PerformRegistrations(registerer);

        }

        public static void Go()
        {
            //Manual Entry
            ConfigureEnTieerApplication();
        }


        private static void ConfigureEnTieerApplication()
        {
            EnTierApplication.Configure(_resolver, _useConfiguredContextType, _contextType);
        }

        
    }
}
