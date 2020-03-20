

using Configuration;
using DIBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utility;

internal static class EnTierApplication{


    public static IDIResolver Resolver{get;private set;}

    public static IDIRegisterer Registerer{get;private set;}


    public static void Initialize(IServiceCollection services){
        Registerer = new MicrosoftDependencyInjectionRegisterer(services);
    }

    public static void Initialize(IApplicationBuilder app){
        Resolver = new MicrosoftDependencyInjectionResolver(app.ApplicationServices);

        ApplicationStart();
    }







    private static void ApplicationStart(){

        //TODO: Later figure a way to give the code user the chance to choos what 
        // configuration provider then want to use.
        Registerer.RegisterSingleton<IProvider<EnTierConfigurations>,MSExtensionsConfigurationsProvider>();


    }

}