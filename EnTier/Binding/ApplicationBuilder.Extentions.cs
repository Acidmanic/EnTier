using EnTier;
using EnTier.Configuration;
using EnTier.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {

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
