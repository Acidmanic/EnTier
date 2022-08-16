Easy NTier (EnTier) Library
===========================

![EnTier](graphics/EnTier.png) __v2__

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
PackageReference Include="EnTier" Version="2.1.0" />
```
or via package manager console:

```
Install-Package EnTier -Version 2.1.0
```

or dotnet Cli:
```
dotnet add package EnTier --version 2.1.0
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
 
  * Note: __EnTier__ NEEDS TO FIND YOUR ID FIELD IN YOUR ENTITY MODEL, THAT WOULD BE POSSIBLE WHEN,
  YOU USE ATTRIBUTE ```AutoValuedMember``` ON THE ID PROPERTY OF THE ENTITY. WHITOUT THAT, OPERATIONS 
  WHICH ARE BASED ON IDS WOULD NOT WORK AS EXPECTED.
  
  
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
This way, everything will work fine but the ```IUnitOfWork.GetCrudRepository<TStorage, TId>()``` 
method still will return the default Crud-Repository for your model, not the Custom Repository 
you just created. To override this behavior, and get your that custom repository, for every usage 
in your EnTier application, you need to do one step more and register your custom repository. 
to do so, in the web applications you can call ```IApplicationBuilder.UseRepository<TStorage,TId,TCustomRepository>()``` 
in your startup file like this:


 ```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //...

            app.UseRepository<PostStg, long, CustomRepository>();
        }
 ```

 and for other types of application like console applications you can just simply call 
 ```UnitOfWorkRepositoryConfigurations.GetInstance().RegisterCustomRepository<TStorage,TId,TCustomRepository>()```.


In ___Example.EntityFramework.CustomRepository___, notice that the https://localhost:5001/Posts/ endpoint 
will return data from CustomRepository, without explicitly using CustomRepository like the other endpoint 
(https://localhost:5001/Posts/custom). That is because ```CustomRepository``` is registered as the 
implementation for ```ICrudRepository``` at the startup. If you delete that line in startup code, then 
https://localhost:5001/Posts/ method will return database data without any manipulation, because it will 
use a default crud repository. But the https://localhost:5001/Posts/custom endpoint will still return 
data from custom repository because it's explicitly using ```IUnitOfWork.GetRepository<TStorage,TId,TCustom>()```.


 * Note: PLEASE NOTE THAT THE ```ICrudRepository.Update(.)``` METHOD IS NOT ACTUALLY WELL-CONFIRMING WITH the __Repository Design Pattern__. 
 IT ONLY EXISTS FOR SO THAT THE _CrudRepository_ OBJECTS HAVE A MORE COMPLETE AND HANDY SET OF METHODS FOR CASES THAT A LITTLE _YAGNI_ WOULD BE MORE BENEFICIAL. 

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

__Stripping Non-Primitive-Properties__

While inserting data into storage at data access layer, in most cases we store a pocco without non-primitive properties. 
So by default, CrudRepositoryBase class will strip away any non-primitive properties from entity by setting thos values 
to null. But In some cases (ie. test repositories and etc.) we might for some reason prefer to insert those values as well. so 
we would want to override this behavior. To do so, We can decorate the insert method with following attributes.

 * ```[KeepAllProperties]```
 * ```[StripAllProperties]```
 * ```[KeepProperty]```
 * ```[StripProperty]```
 
 These attributes can be placed on ```ICrudRepository.Add(.)``` method, or in higher lever, on ```ICrudService.Add(.)``` 
 method or in higher level on ```Controller.Add(.)``` method. Basically on any method in call chain from Controller to 
 repository. In General, ```[StripProperty]``` attributes supersede ```[KeepProperty]``` attributes. And between 
 ```[KeepAllProperties]``` and ```[StripAllProperties]```, the latter supersedes the former.
 
 You can mark any number of entity types with ```[KeepProperty]``` or ```[StripProperty]```, while using these attributes:
 
 ```c#

    [StripProperty(typeof(StrippingProperty1),typeof(StrippingProperty2))]
    [KeepProperty(typeof(KeepingProperty1))]
    public Model Add(Model value){
    
    //...

    }
```  

 * Note: You can change these behaviors while driving from ```CrudRepositoryBase``` class by overriding the add method.

 * Note: While driving from ```CrudRepositoryBase```, you can use ```StripNonPrimitives()``` method which stripes all non-primitive 
properties, and ```StripMarkedSubEntities()``` method which strips marked non-primitive properties regarding the delivered 
attributes to the call chain.

* Note: Since InMemoryCrudRepository and JsonFileCrudRepository, are mainly purposed for tests and demoes, these repositoreies 
will keep and actually insert all non-primitive properties by overriding the add method and adding ```[KeepAllProperties]``` 
attribute on it. This way you can override this behavior again inside your code.

```c#
     [KeepAllProperties()]
    public override TStorage Add(TStorage value)
    {
        return base.Add(value);
    } 
``` 

you can achieve same result by overriding the ```Add()``` method and calling ```base.Insert()``` instead, for less overhead.

Custom Services
-----

You can add your custom service instead of the default Crud service that EnTier automatically will create for you. 
Your custom service must implement ```ICrudService<..>``` interface. The you can 

  * Pass your service instance to Controller's constructor

  Or (Recommended)  
   * Creat a contract (interface/abstraction) for your custom service and register your custom service for your contract
   via your DI container. Then inject your contract into your controller's constructor. and pass the injected object into 
   base constructor.
   
   
 This way, your custom service will be used in EnTier calls and you will be able to manipulate the service's behavior in 
 any way you need.
 
 
 *  NOTE: You can implement your custom service, by extending the class ```CrudServiceBase<>``` easier.
 *  NOTE: If you are injecting your custom service into your controller, DO NOT FORGET TO REGISTER it on your container.
 
 Data Access Regulation
 ----------------------
 
 
 In some cases you might need to have some kind of regulation on your data before its being processed or stored. 
 For example you might be receiving a chunk of data from the user to be inserted into Database, and it's crucial 
 that a specific field of your data be filled, or an id must already be existing in the database or thing like this.
 
 The DataAccessRegulation helps to implement this in __Service__ level. An ```IDataAccessRegulator<TDomain,TStorage>```, has a 
 ```Regulate(TDomain model)``` method. This method takes a domain value and returns a ```RegulationResult```. A RegulationResult 
 object, has a __Status__ field, a ```TModel``` field containing domain model, and a ```TStorage``` field containing mapped 
 storage model. (So in practice you might need to inject your mapper into your regulators). 
 
 The Status field can be rejected, Accepted or Suspicious. The paradigm is to investigate received domain-model, 
 and provide a valid storage counterpart for it and putting both this values into a ```RegulationResult``` object 
 with _Ok_ Status. 
 
  * a RegulationResult with status = Ok, Means that both Domain and Storage fields are trustable and safe to use.
  
 If the received domain model, is somehow un acceptable, The ```Regulate``` method will return a RegulationResult object 
 with _UnAcceptable_ status, meaning that the data is rejected and Domain and Storage fields are not trustable.
 
 * a RegulationResult with status = UnAcceptable, Means that data is rejected and nighter Domain or Storage fields are not valid.
 
 In some cases, the received data might be incorrect but you find it safe to fix the data and use it. In such cases the 
 ```Regulate``` method will produce a fixed and valid storage model, and put it into a RegulationResult object with the 
 _Suspicious_ status.
 
 * a RegulationResult with status = Suspicious, Means that the Storage field is usable but there was a problem with the received data.
  
  
  You can create Regulators by implementing ```IDataAccessRegulator<TDomain,TStorage>``` or extending 
  ```DataAccessRegulatorBase<TDomain,TStorage>```. Regulators can be injected and used in your services.
  
  __Use Regulators in Default CrudServices__
  
  If you have a regulator for your storage/domain model pairs, you can make EnTier to use your regulator before performing 
  __Create__ and __Update__ operations. To do that, you just need to pass an instance to the base controller's constructor.
  The better practice would be to define a contract for the regulator, Registering the regulators for its contract in DI 
  Container. and injecting it into the controller to be passed to base controller. ___Example.Regulation___ project shows such a 
  use case. For simplicity, this example only uses one Model type for all layers.  

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
