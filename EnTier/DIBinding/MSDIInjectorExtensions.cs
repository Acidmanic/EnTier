



using System.Diagnostics;
using System.Reflection;
using Configuration;
using DIBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utility;

public static class MSDIInjectorExtensios{




    public static IServiceCollection AddEnTierServices(this IServiceCollection services){

        EnTierApplication.Initialize(services);

        return services;
    }


    public static IApplicationBuilder UseEnTier(this IApplicationBuilder app){

        
        
        ReflectionService.Make().CacheCurrent();
        
        var s = new StackTrace();

        var caller = s.GetFrame(1);

        var ass = caller.GetMethod().DeclaringType.Assembly;

        ReflectionService.Make().Cache(ass);

        EnTierApplication.Initialize(app);

        return app;
    }


}