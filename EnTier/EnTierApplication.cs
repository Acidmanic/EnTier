

using Configuration;
using DIBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utility;
using System;
using Microsoft.EntityFrameworkCore;
using Channels;
using Generics;
using Service;
using Repository;

internal static class EnTierApplication{


    public static IDIResolver Resolver{get;private set;}

    public static IDIRegisterer Registerer{get;private set;}
    public static bool IsContextBased { get; private set; }

    public static void Initialize(IServiceCollection services){
        Registerer = new MicrosoftDependencyInjectionRegisterer(services);

        PerformRegistrations();
    }

    public static void Initialize(IApplicationBuilder app){
        Resolver = new MicrosoftDependencyInjectionResolver(app.ApplicationServices);
        
        ApplicationStart();
    }



    private static void PerformRegistrations(){
        //TODO: Later figure a way to give the code user the chance to choos what 
        Registerer.RegisterSingleton<IProvider<EnTierConfigurations>,MSExtensionsConfigurationsProvider>();

        Registerer.RegisterSingleton<IGenericBuilder<IService>,GenericServiceBuilder>();

        Registerer.RegisterSingleton<IGenericBuilder<IRepository>,GenericRepositoryBuilder>();

        Registerer.RegisterSingleton<IGenericBuilder<IUnitOfWork>,GenericUnitOfWorkBuilder>();

        Registerer.RegisterSingleton<IGenericBuilder<IProvider<IUnitOfWork>>,GenericUnitOfWorkProviderBuilder>();
    }



    private static void ApplicationStart(){

        IsContextBased = ReflectionService.Make().IsAnyExtensionFor<DbContext>();

        ChannelsService.Make().UpdateChannels();

    }


}