using EnTier;
using EnTier.Configuration;
using EnTier.Bootstrap;
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

            Boot.Strap(app);

            Boot.Strap(configurer);

            return app;
        }


    }
}
