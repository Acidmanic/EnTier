


using AutoMapper;
using DataAccess;
using Mapping;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class RegisterServices
{
    

    public static void AddApplicationServices(this IServiceCollection services){


        services.AddTransient<ILoginService,DefaultLoginService>();
        
        services.AddSingleton(sp => AutomapperBootstrap.CreateMapper());

        services.AddTransient<IProvider<GenericDatabaseUnit>,GenericDatabaseUnitProvider>( );

        services.AddTransient<IUsersService,UsersService>();

        services.AddTransient<IObjectMapper,Automapper>();
    }
}