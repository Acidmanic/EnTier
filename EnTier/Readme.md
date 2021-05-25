







Manipulate Crud Service
--------

Override Acquirer CrudService


Manipulate Mapper
----------

Through The Injection

Declare Unit Of Work
------------

Each application will has its own unit-of-work implementation and repositories, 
isolating the data-access layer. If you dont define any unit of work, 
EnTier will create an ```InMemoryUnitOfWork``` which will produce ```InMemoryCrudRepository```s. 
But in real case scenario you will create your own UnitOfWork class and inject 
it into CrudControllers ___through the constructor___. For this to work you need 
to implement ```IUnitOfWork``` in it, so the CrudControllers use the same data access
 layer as your main application. You also need to deliver this unitOfWork instance 
 through your DI, so you need to override one of the constructors in the CrudControllerBase 
 which takes IUnitOfWork argument.

There are two Builtin data access layers shipped with library:

1) __InMemory__: Suitable for unit tests, contract/integration tests and etc..
2) __JsonFile__: Suitable mostly for demos.

3) __EntityFramework__: This one is not actually within the EnTier nuget package,
 since it adds EntityFramework dependency to your application, this part is packaged 
 separately, you can get it from NuGet.org. (EnTier.DataAccess.EntityFramework)
 
If your using Dotnet Core's builtin DI, To use one of Builtin UnitOfWorks (InMemory,JsonFile,EntityFramework)  
you can either register your IUnitOfWork implementation of choice into IServiceCollection using 
AddTransient/AddSingleton/etc methods. Or you can Use ```AddJsonFileUnitOfWork()``` 
, ```AddInMemoryUnitOfWork()``` or ```AddEntityFrameworkUnitOfWork()``` extension methods.

Directly Supported DI
================

So far, Only dotnet core's builtin DI system is supported. 

Changing the DI, means methods like AddAutoMapper() are not there and you need to register 
```AutoMapperAdapter``` for ```EnTier.Mapper.IMapper``` on your DI system manually. 

Tests
======

For tests, you can wrap your fixture preparation codes, into Fixture classes. Fixture classes 
does not need to extend or implement any abstraction. And they can have constructor injections. 
 
 * Fixture class will be instantiated through DI
 * Fixture class can declare one or more ```Setup(.)``` methods with arguments 
 of type ```ICrudRepository<TStorage,TId>```. These methods will seed 
 the test data.
 * The order of execution of Fixture classes, would be the same
  order they has been added.
  
There are some points to take into consideration:

 * Fixture mechanism is for unit tests, not production.
 * It will deliver crud repositories based on registered UnitOfWork 
 on the DI. (Currently only supports dotnet core builtin DI). 
 * If no UnitOfWork has been registered, it will use InMemoryUnitOfWork 
 by default.  

 * Note: When there is no UnitOfWork Registered on DI system, EnTier will 
 instantiate an ```InMemoryUnitOfWork``` by default, __BUT__ if you intentionally
  want to use any UnitOfWork (any data-access) for your tests, including InMemory 
  data-access, you have to explicitly register it in your DI for Fixture mechanism 
  to recognize it and work properly. 

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
 =========
 
 Type of Ids _Can_ be different in each layer for an entity. But 
 in order to use different Id Types you need to take in
  consideration 
  1) Mapping of Entities,
  2) Mapping of Ids.
  * The Builtin Mapper does not support configurations, therefore it
   cannot handle mapping different types for ids.
   
  The Example: __Example.AutoMapper__ shows a use-case with storage and domain 
  using Guid for id type and transfer objects having string Ids.
  
  