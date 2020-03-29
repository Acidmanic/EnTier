



using System;
using System.Diagnostics;
using System.Reflection;
using EnTier.Configuration;
using EnTier.DIBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using EnTier.Utility;
using EnTier;

namespace EnTier.DIBinding{


    public static class MSDIInjectorExtensios
    {

        public static IServiceCollection AddEnTierServices(this IServiceCollection services)
        {

            EnTierApplication.Initialize(services);

            return services;
        }


        public static IApplicationBuilder UseEnTier(this IApplicationBuilder app, Action<IEnTierApplicationConfigurer> configurer = null)
        {



            ReflectionService.Make().CacheCurrent();

            var s = new StackTrace();

            var caller = s.GetFrame(1);

            var ass = caller.GetMethod().DeclaringType.Assembly;

            ReflectionService.Make().Cache(ass);

            if (configurer != null)
            {
                configurer(new EnTierApplicationConfigurer());
            }

            EnTierApplication.Initialize(app);


            return app;
        }


    }
}



