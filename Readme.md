Easy NTier (EnTier) Library
===========================

![EnTier](graphics/EnTier.png) __v2.0.0__

Changes From v1.0.x
===============
There were epic major changes from version 1 to 2. I will try to provide guides to switch to 2.0 
as soon as i can.


About
=====

This library gives you a pre-implementation of  an N-Tier crud application. So you 
focus on writing other functionalities of your code. By Inheriting ```CrudControllerBase``` 
you basically have a Controller using a service using UnitOfWork pattern to get a 
repository to perform restful crud operations and all are plugged together and working.

 * You can create a CrudController then add other endpoints to it if necessary.
 * You can inject any service you need into your Controller
 * You can Create your service from scratch, also you can extend ```CrudService``` 
 which already has crud operations implemented, then add additional functionalities 
 you need to it.
 * Since best practice for Uni-of-work pattern implies to have __One__ UnitOfWork 
 per application, if you want to implement your own UnitOfWork class, you can implement 
 ```IUnitOfWork``` interface or you can extends the ```UnitOfWorkBase``` class to create your 
 own unit of work.
 * For Repositories, it's the same; you can write them from scratch, implement 
 ```ICrudRepository``` or extend (or wrap) ```CrudRepository```. 

 
Getting started
===============
 
Adding the library
------------------


 First you will add the package. It's available on 
 [NuGet.org](https://www.nuget.org/packages/EnTier/). you can add the reference
  to your &lt;project&gt;.csproj:
  
```
PackageReference Include="EnTier" Version="2.0.0" />
```
or via package manager console:

```
Install-Package EnTier -Version 2.0.0
```

or dotnet Cli:
```
dotnet add package EnTier --version 2.0.0
```

Running a simple example
------
 
The simplest use-case of the library would be 
 1) create and new web-api dotnet core project 
 2) create a model
 3) add a crud controller by extending ```CrudControllerBase``` , and adding a ```[Route(...)]``` 
attribute to it.
 
 Done!
 The example project __Example.SingleLayerEntity__ implements such use case. This way 
 EnTier will create and use an ```InMemoryUnitOfWork```, which itself produces in-memory 
 repositories and use them with ```CrudService```(s).
 
Using JsonFile data access layer
-------
 
 In-Memory data access layer is suitable for testing. But you can change it to JsonFile 
 data access layer easily by calling ```services.AddJsonFileUnitOfWork();``` at the 
 Startup class. This way you will use builtin ```JsonFileUnitOfWork``` which produces 
 ```JsonFileCrudRepository```(s). This data access, persists data in json files kept 
 in a directory called __JsonDatabase__ where your executable file is.
 
 This data access layer might be more suitable for instant demos or such use cases.
 
 The example project __Example.JsonFile__ shows this implementation. In this example 
 also different models are used for Transfer (Dtos), Domain and Storage. so EnTier 
 uses its internal Mapper to map objects. Later you will see how to use another 
 mapper instead. 
 
Using EntityFramework
----------    
 
 Same as Json data access layer, you can switch your data access layer to EntityFramework 
 just by calling  ```services.AddEntityFrameworkUnitOfWork(context);``` at 
 startup class. This method takes a DbContext as argument, and uses this db context to 
 perform all crud data operations.
 
 for this to work you should 
  1) Have Entity Framework packages installed 
  2) Add extension package: 'EnTier.DataAccess.EntityFramework' which is also available on NuGet.
  3) You should have your ef migrations created and applied. 
  
   * __Important note__: do not forget to add DBSets<Entity> properties for each entity  
    into your DBContext class. Otherwise Entity Framework would not recognize your entities
    and they will not be included in your migrations.
    
 The example project __Example.EntityFramework__ shows this implementation. This example also 
 uses isolated models for each layer.
  


Using a Mapper
----------

The builtin mapper, is very limited and is not capable of handling a production situation. 
since mappers are being added using injection, you can use any Mapper of your choice 
just by wrapping it in an implementation of ```IMapper``` which is a very simple interface.

This is already done for [AutoMapper](https://github.com/AutoMapper/AutoMapper) package, so 
if you prefer to use AutoMapper, just add its [AutoMapper package](https://www.nuget.org/packages/AutoMapper/),
then add AutoMapper extension package: 'EnTier.AutoMapper' from NuGet. And add AutoMapperAdapter 
to Ioc container for IMapper. 

```c#
    services.AddAutoMapper(config => ...);
``` 
 
 The example project __Example.AutoMapper__ shows this implementation. This project also shows
  how to have different types for Id property on Entity models using Mapper. 


Custom Repositories
-------------------


When you implement your own ```ICrudRepository```, you can get your Custom repository 
in your services by calling ```IUnitOfWork.GetCrudRepository<TStorage, TId, TCustomCrudRepository>()```.
This way everything will work fine but ```IUnitOfWork.GetCrudRepository<TStorage, TId>()``` 
method still will return the default Crud-Repository for you model, not the Custom Repository 
you just created. To override this behavior, and get your custom repository used for every usage 
in your EnTier application, you need to do one step more and register your custom repository. 
to do so, in the web applications you can call ```IApplicationBuilder.UseRepository<TCustomRepository>()``` 
in your startup file like this:


 ```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...

            app.UseRepository<ICrudRepository<PostStg, long>, CustomRepository>();
        }
 ```

 and for other types of application like console applications you can just simply call 
 ```UnitOfWorkRepositoryConfigurations.GetInstance().RegisterCustomRepository<TAbstraction,TRepository>()```.


In ___Example.EntityFramework.CustomRepository___, notice that the https://localhost:5001/Posts/ endpoint 
will return data from CustomRepository, without explicitly using CustomRepository like the other endpoint 
(https://localhost:5001/Posts/custom). That is because ```CustomRepository``` is registered as the 
implementation for ```ICrudRepository``` at the startup. If you delete that line in startup code, then 
https://localhost:5001/Posts/ method will return database data without any manipulation, because it will 
use a default crud repository. But the https://localhost:5001/Posts/custom endpoint will still return 
data from custom repository because it's explicitly using ```IUnitOfWork.GetRepository<TStorage,TId,TCustom>()```.



__Custom Repositories and constructor injection__


When you write a custom repository, it will be instantiated by your UnitOfWork. So That determines the limitations of 
injection.

* If you are using the builtin _Json DataAccessLayer_ or _InMemory DataAccessLayer_, then there will be no 
injection available for your custom repositories. Your custom repository must have a parameter-less constructor.

* If you are using _Entier.DataAccess.EntityFramework_, you can have any number of ```DbSet<TStorage>``` parameters 
injected into the repository through the constructor. the __EntityFrameworkUnitOfWork__ will find and instantiate and 
deliver these. But just make sure you have such db-sets as properties of your DbContext. Using this feature, you can have 
access to any db-set in your repository, create required queries inside the repository and return in-memory objects 
(vs queriables) to your services.

Tests!
------

One thing this library solves for me, is to ease decoupling of data access layer from services. 
You can use for example EntityFramework for production, and in UnitTest os services, Contract tests 
and some of integration tests (basically any type of test that is not supposed to cover your data-access 
code itself), you can use InMemory data access. 

EnTier also provides a very simple Fixture creation mechanism which will pre populate your data base
 easily. To do so you just 
 
 1) __Add a Fixture class__: 
     Its a normal class which supports constructor injection. Plus you can declare any number of 
     methods called ```Setup(...)```. these methods will be executed before application starts. 
     setup methods can have any number of ```IRepository<TStorage,TId>``` arguments. These repositories 
     will be injected into method before being called, based on the unit of work you are using in your 
     test project. 
     
 2) __Use Fixture classes at Startup class__: 
     By calling ```app.UseFixture<ExampleFixture>();``` in ```Configure(IApplicationBuilder app, ...)``` method 
     of your startup class, you can use each Fixture class.
     
 Example project __Example.Test__ shows an integration-like test with EnTier and InMemory data access layer. 
 it uses [xUnit](https://github.com/xunit/xunit) as test frame work and tests the project 
 __Example.JsonFile__
 
  
     
        
 * Note: If you change your DI, from a DI other than dotnet's builtin DI, the UseFixture method 
 will change slightly regarding the DI/Container you are using. 

Dependency Injection / Ioc Container
================

By default, EnTier supports and uses dotnet core's builtin Di. But you can use it with 

 * __CastleWindsor__: by adding the package __EnTier.DependencyInjection.CastleWindsor__ from 
nuget.org. 
     * CastleWindsor may or may not be registered on ```IServiceCollection```, so for adding 
     fixtures, to make sure it works correctly, you should call ```AddFixtureWithWindsor<T>()``` 
      on your container object (```IWindsorContainer```).
 * __Unity__: by adding the package __EnTier.DependencyInjection.Unity__ from nuget.org. 
     * For adding fixtures, you would call ```app.AddFixtureWithUnity<T>()``` at 
     ```Configure(IApplicationBuilder app,...)``` method in your startup class. 

 * The example project __Example.CastleWindsor__ shows a use case of EnTier alongside Castle.Windsor Ioc.
 
 * The example project __Example.Unity__ shows a use case of EnTier alongside Unity Container Ioc.
 
 * Changing the DI, means methods like ```services.AddAutoMapper()``` are not there and you need to register 
```AutoMapperAdapter``` for ```EnTier.Mapper.IMapper``` on your DI system manually. Its also 
the same for ```services.AddEntityFrameworkUnitOfWork()```, and you should register an instance
 of ```EntityFrameworkUnitOfWork``` (or any custom UnitOfWork implementation) for interface ```IUnitOfWork``` manually.

Notes
=========

 * Builtin Mapper is no good for production, 
 its just for library to work out of box to ease the 
 process of familiarizing and first usages. Using 
 Automapper is Highly recommended.  
 
 * Do not forget to add ```[ApiController]``` and ```[Route(...)]```,
  attributes on controllers, otherwise it might take a long time to figure the cause of 
  silent bugs!

  
 Id Types
 ------
 
 Type of Ids _Can_ be different in each layer for an entity. But 
 in order to use different Id Types you need to take in
  consideration 
  1) Mapping of Entities,
  2) Mapping of Ids.
  * The Builtin Mapper does not support configurations, therefore it
   cannot handle mapping different types for ids.
   
  The Example: __Example.AutoMapper__ shows a use-case with storage and domain 
  using Guid for id type and transfer objects having string Ids.
  
  
  
```text
  I Hope this saves some of your time
  Regards
  Mani
```
