
Easy NTier (EnTier) Library
===========================


![EnTier](graphics/EnTier.png)

EnTier is a library for easily implementing an NTier application. It gives you pre-implementations 
for Controllers, Services and repositories. In simplest use case, after writing models, you can 
create a controller for Storage/Domain/Transfer set of models, by extending EnTierControllerBase. 
This controller, will have restful CRUD api methods implemented.

Other than that, you can:

*   Use your own service(s) in your controller
*   Use automatically provided Respository in your services
*   Use your own Repositories in your services
*   Use a builtin Context for application (FileContext, EntityFrameworkContext, InMemoryContext )
*   Use your own implementation of IContext
*   Use Automapper as your mapper
*   Use EnTier's default mapper (very nive implementation, recomended only for tests)
*   Use Services, in a different presentation for example a console aplication.

Main components you might be deal with, are:

*   Controller
    *    web api presentation
*   Service
    *    Main bussiness logic implementations
*   UnitOfWork 
    *   Provides proper Repository for a service.
*   Repository
    *   Performs CRUD operations on Application's Context
*   Context
    *   Wraps Infrastructure which communicates with DataStorage.


Start Simple
============

In Simplest scenario, having your model classes in hand, you can extend the __EnTierControllerBase__ class. Also 
You should start and configure EnTier, which is easy. If you're application is a mvc web api (.net core), you 
should add these lines of codes in your Startup class:

```C#
        public void ConfigureServices(IServiceCollection services)
        {
            // ...

            services.AddEnTierServices();

            // ...
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // ...

            app.UseEnTier();

            // ...
        }
```

after that, you should have a restful api end point named after your controller. You can Add a [Route("uri")] 
attribute to your controller class to use dotnet custome routing...

created api endpoint will provide these functionalities:


|    API Endpoint                       | Method    | Request Payload           |  Description                          |EnTierControllerBase Method        |
|:-------------------------------------:|:---------:|:-------------------------:|:-------------------------------------:|:---------------------------------:|
|{base-url}/{uri}/{controller-name}     | GET       |-                          | Returns all availabe dara frim thread |```List<Entity> base.GetAll()```   |
|{base-url}/{uri}/{controller-name}/{id}| GET       |-                          | Returns Asked Entity if exists        |```Entity base.GetById(Tid Id)```  |
|{base-url}/{uri}/{controller-name}     | POST      |Entity Object Without Id   | Creates and returns a new Entity      |```Entity base.CreateNew(Entity entity)```|
|{base-url}/{uri}/{controller-name}     | PUT       |Entity Object With Id      | Updates An Entity                     |```Entity base.Update(Entity entity)```|
|{base-url}/{uri}/{controller-name}/{id}| DELETE    |-                          | Deletes An Entity                     |```Entity base.DeleteById(Tid id)```|
|{base-url}/{uri}/{controller-name}     | DELETE    |Entity Object With Id      | Deletes An Entity                     |```Entity base.DeleteByObject(Entity entity)```|

So in summary, You will write your models, setup EnTier Application, and Extend EnTierControllerBase. This will automatically create proper Service and Repository 
regarding presented generic types for storage,domain and transfer models. It's possible to use the same type for all three types, but it's not recommended. 
Besides that, you can manupilate the behavior of controller about 1) which one of these methods above gets implemented, (default is all) and 2) Determine which 
fields of your storage model, gets loaded Eagerly. Next section show how to do these manupilations.



Manipulating the Controller
=======================





you can also write and use you own service to be used in this controller. Next section will show how to do this.



Hook in your own Service
========================


A pre-built Service which gets used in an EnTierController, provides 4 main CRUD operations. You can write your own service.




                                    [ Controller ]
                              ____________|_____________
                             |                          |
                        [ Service ]                 [ Service ]
                             |__________________________|
                                          |
                                          |
                                    [UnitOfWork]
                                          |
                           _______________|_______________
                          |               |               |
                   [ Repository ]  [ Repository ] ... [ Repository ]
                          |               |               |
                          |_______________|_______________|
                                          |
                                      [Context]
                                           



*   __EnTierControllerBase__: This is a base class for creating a new controller based on a model. 
    _EnTierControllerBase_ takes 4 generic arguments:
    *   StorageEntity: This represents the Type of data access layer's model.
    *   DomainEntity: This represents the Type of bussiness layer's model.
    *   DataTransfer: This represents the Type of presentation layer's model.
    *   Tid: This represents the Type of id field in Entities. This Generic argument
        is Optional, If not provided, it's default would be Long.
*   __ServiceBase__: This class can be 



Context
=======


A Context Will provide the access to data source. For different Datasources, 
we can have different Contexts accordingly. for example, The class _DatabaseContext_ 
is Context that provides access to SqlDatabases using EntityFramework. Each Channel 
can have its own Context. you can use different context for each channel.
Contexts should be Provided when channel is created. If You dont specify your context 
in configurations, EnTier application will try to find a context for your channel 
depending on channel's storage entity. and if it does not find it, a NullContext will 
be used.

Hooking In
==========


*) Tools in hand:
-----------------

__To Plug a service to a controller__

1) Constructor Injection In Controller (Registered in DI)
2) AutoInjection using InjectionAware attribute (Registered in DI)
    *** RepositoryBase, needs an IDataset object in it's constructor
    *** This will be satisfied correctly when UnitOfWork creates the
    *** Repository. but when your sending you implementation to injection,
    *** your injector, would not know about dataset object. so if you are
    *** using auto injection to plug a repository which needs the dataset 
    *** be injected on its constructor, inject IDatasetAccessor instead
    *** and you can get the dataset from it. __IMPORTANT:__ it only is valid
    *** during the constructor method. afterwards it returns null.
3) By Convention (Mathch Interface And Non Argumented Constructor)
Otherwise) Generic one would be used.


__Internal dependencies__

 ____________      _________      ___________      __________
| Controller |<---| Service |<---|           |<---|Repository|<------|
|____________|    |_________|    | UniOfWork |<---|Repository|<------|
                                 |___________|<---| Context  |<---|Dataset|
                                                  |__________|<---|Dataset|

__Development rules__

*) Repository and Service base classes Should not have any argument in constructors
*) Generic classes are just exposing base classes to be instanciatable, but they cant be seen outside of assembly.

__Channels Management__

*) For each controller, There is only one main Entity wich is it's Transfer Entity
*) Alongside each controller, a channel will be defined.
*) Channels should be accessable in Controller and corresponding UnitOfWork
*) Channels should be able to provide Services and Repositories using generic argumens of controller.
*) Channel should keep the result of which context should be used for the channel. but it's not its responsibility to provide it. Context depends on settings and availability of DBContext.




1) Hook In By Direct Injection
------------------------------
Inject Your Service In Your Controller


2) Hook In By Indirect Injection
--------------------------------
Inject Your Service In Your Controller
Inject Your Repository In Service 


3) Hook In By Convention
------------------------
Plug Your Service In Your Controller
Plug Your Repository In Your Service



4) Direct Channel Design
------------------------
Plug Service In Controller
Plug Repository In Service



Startup
========



[Binders]   |
...         | ----->   [Bootstrap] ----> EnTierApplication
[Starters]  |

Binders / Starters
------------------

(EnTier/Binding/Abstractions)
(EnTier/Startup)

Binders are extension classes which hook into starter classes for different environments. 
they might receive configurations and etc when main application starts. When there is no 
Binder to start the application, like when we're in a console application, then an starter 
will be used to do the same job. Both binders and starters, are a link between client application 
startup and Bootstrap.


Bootstrap
----------

(EnTier/Startup)

The Bootstrap, has the reponsibility to determine what configuration and initial dependencies 
has been delivered from client application startup routine, and Setup the EnTierApplication based 
on it.


EnTierApplication
-----------------

(EnTier/EnTierApplication)

This is a static class which is a reference for application-wide configurations. after this class has 
been configured by Bootstrap, it will perform internal configurations and initializations such as 
cachings and etc., and then the applications is started.