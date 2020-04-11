
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

               Table 1: Pre Implemented Crud Methods 


So in summary, You will write your models, setup EnTier Application, and Extend EnTierControllerBase. This will automatically create proper Service and Repository 
regarding presented generic types for storage,domain and transfer models. It's possible to use the same type for all three types, but it's not recommended. 
Besides that, you can manupilate the behavior of controller about 1) which one of these methods above gets implemented, (default is all) and 2) Determine which 
fields of your storage model, gets loaded Eagerly. Next section show how to do these manupilations.


Generic Types Description
=========================

All base classes and interfaces of main components of EnTier library use number of these 4 generic 
types. When you extend a base class or implement an interface, you will get around undrestanding these 
types and what they represent:


*   StorageModel: This represents the Type of data access layer's model.
*   DomainModel: This represents the Type of bussiness layer's model.
*   TransferModel: This represents the Type of presentation layer's model.
*   Tid: This represents the Type of id field in Entities. This Generic argument
    is Optional, If not provided, it's default would be Long.




Manipulating the Controller
===========================

*   __Select which CRUD method be implemented__:

    For this, you should use controller configuration builder. It's aavailable on overridable mthod 
    ```OnConfiguringController(ControllerConfigurationBuilder builder)```. The builder object allows 
    you to choose which of one methods in _Table 1_ to be implemented.


*   __Control Eager Loading__:

    ⚠️ This only works for EntityFramework Context. For other contexts, it just has no effect. 

    You can use either EagetScoperManager object or Eager attribute to mark what fields of which Entity, should be loaded 
    eagerly in every block of code. You can use EagerScopeManager object in using block. and mark fields for that block like this:

    ```C#
        //...
        using(var scope = new EagerScopeManager())
        {
            //Clear any fields 
            scope.Mark<User>();
            //Include Posts field to be loaded eagerly
            scope.Mark<User>(q => q.Include(u => u.Posts));

            //... any user loaded here, will have it's posts loaded eagerly
        }
        //...
    ```

    Another way for marking eager fields, is to use Eager attribute. You can put any number of Eager attributes on either a class or a method. 
    Naturally putting an eager Atrribute on a class causes any code inside the class to deal with fields the ways specified 
    in the attribute. The same for a method. In many cases, these blocks and attributes, will be nested. Always the most inner 
    marking, will be effective for each Entity.

    Makring with attributes:

    ```C#

        [Eager(typeof(User),nameof(User.Posts))]
        UsersSummary AnalyseUsers()
        {
            // Load users with all their posts loaded eagerly
            var users = Service.GetAll();
            //.. do something 
		}
    ```

You can also write and use you own service to be used in this controller. In such case, marking eager fields will be the 
same as described above. Next section shows how to add your own services


Hooking in your own Services
============================


Each Controller atleast needs one service which provides access to the Domain Entity. This domain entity is the one that is 
introduced to controller by Generic arguments. By default, controller will request such service from EnTier's component 
producer. and will use this service for basic CRUD operations. In cases that you need more logic than just CRUD to be 
applied on data, you would need to write your own service. Ideally you would like to write only your additional logic. For 
that, you can inherit ```EnTier.Service.ServiceBase<,,>```. Then you would need to 1) Access data using repositories. 2) 
Hook your service into your controller.

1) Access data using Repositories (Unit Of Work)
------------------------------------------------

Repositories are not available directoly in services. Instead an implementation of __Unit Of Work__ is being used. Therefor 
you will get proper repository from a ```UnitOfWork``` object. A UnitOfWork, has a ```GetRepository<TDomain,TStorage,Tid>()``` method which provides 
the repository based on entity generic types. UnitOfWork objects are disposible and the disairable way for using theme is to 
create, use immidiately, then dispose them. The ServiceBase class, has a property Named ```DBProvider```. This DbProvider 
can create a UnitOfwork object when needed. You can use this object in a using block. Also it's possible to keep the object 
in a private field in your service and dispose it when your service is done, but it's not recommended. The following code shows 
an example of accessing data inside a Service:


```C#
    
    //...
    public User AddUser(DomainModels.User domainUser)
    {
        var storageUser = Mapper.Map<StorageModels.User>(domainUser);

        using(var db = DbProvider.Create()){

            var repository = db.GetRepository<StorageModels.User,long>();

            storageUser = repository.Add(storageUser);
                
            db.Compelete();
        }

        return Mapper.Map<DomainModels.User>(storageUser);
    }
    //...
```

| ⚠️ Note                                                            |
|:-------------------------------------------------------------------|
| ⚬ Both __EnTierControllerBase__ and __ServiceBase__, have a Mapper property. this property provides the functionality needed to map entities. This will be described more in its own section.|
| ⚬ A service, exposes data with Domain Entities. but it receives data from repositories in Storage Entities. (The same way Controllers receive Domain Entities from services, and they return mapped Transfer entities.)|


2) plugging your Service in
---------------------------

* 2-1 __By Direct Injection__: The most straight forward way for having your service in your controller, is to directly inject its 
interface to your controller, and register the concrete service class in your DI's registerer.

* 2-2 __By Auto Injection__: If you want to keep your constructor thin, or for any other reason you dont want to inject in 
controller, you can use Autoinjection attribute which is named ```InjectionEntry```. There is two way to use this attribute. 
first is to put it on the interface that you are registering in your DI. The second way is to put it on implemented Service 
and pass the type of your interface to it. The second way offcourse gives a more readable code:
```C#

    /*
        Example Of using InjectionEntry on Interface
    */


    [InjectionEntry]
    interface IUsersService
    {
    
        //Additional logic
        public List<DomainModels.User> GetVIPUsers();
	}

    class UsersService:ServiceBase<StorageModels.User,DomainModels.User>,IUsersService
    {
        
        //...

        public List<DomainModels.User> GetVIPUsers()
        {
            //Implementation  
		}
	}
```


```C#

    /*
        Example Of using InjectionEntry on Implementation
    */

    interface IUsersService
    {
    
        //Additional logic
        public List<DomainModels.User> GetVIPUsers();
	}

    [InjectionEntry(typeof(IUsersService)]
    class UsersService:ServiceBase<StorageModels.User,DomainModels.User>,IUsersService
    {
        
        //...

        public List<DomainModels.User> GetVIPUsers()
        {
            //Implementation  
		}
	}


```

* 2-3 __By Convention__: If you provide an implementation of ```IService<TDomain,Tid>``` in your code which has a 
constructor with no arguments, this class will automatically be used in your controller. 

| ⚠️ Note                                                            |
|:-------------------------------------------------------------------|
| ⚬ Controller, will search for it's main service, based on TDomain and Tid.|
| ⚬ These 3 methods to plug a service, can be used alongside each other. but if you provide two services for the same <TDomain,Tid>, Convention method will be overrided by AutoInjection. And AutoInjection will be overrided by DirectInjection.|
| ⚬ Try to keep your code uniform and consistant by preventing to use different methods all togetter as much as possible.|
| ⚬ You can implement IService interface from scratch without extending ServiceBase, but this way you wount have access to pre implemented codes for CRUD operations, UnitOfWork, Repsitories and etc.|


Hooking In Your Repositories
============================

In Some cases, you might need to extend existing repositories or implement your own. This is also possible and it's mostly 
the same routine with adding services. You can extend ```EnTier.Repository.RepositoryBase``` or implement ```EnTier.Repository.IRepository```. 
to hook your repositori into the EnTier application you can use AutoInjection (InjectionEntery attribute) or conventions 
(Implement IRepository with a constructor with no arguments). Using DI for Repositories is nuot supported in this version.

Choose/Add Context
==================

A Context Will provide the access to data source. EnTier library has three type of builtin contexts: __InMemoryContext__, 
__FileContext__ and __EntityFrameworkContext__. You can choose context type for your application in startup code. 
__InMemoryContext__ provides an in-memory database and it's convinient to use for development tests. __FileContext__ creates 
a directory in your application's execution directory, named __SerialziedDatabase__ and stors and retrives your data to/from 
json files inside this directory. __EntityFrameworkContext__, will search for your DbContext and uses that to connect to your 
database using EntityFramework.

Choosing Context type in startup code
-------------------------------------

For a .net core mvc application it would be like this:

```C#

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        //...

        app.UseEnTier(c => c.SetContext<FileContext>());

        //...

    }
```

and for a Console application it would be like:

```C#

    static void Main(string[] args)
    {
        //...

        Scratch.Start(c => c.SetContext<FileContext>());

        //...
    }
```


Adding new Context Type
-----------------------

To Add a new context, You need to implement IContext and IDataset interfaces. Your context's 
constructor should not take any arguments. Then you can select your Context type for applciation 
the same way you used for builtin Context types.

Application Types Supported
===========================

In This version, the library has been tested for .NetCore Mvc application and Console applications. 
You might be writing a console tool which uses data or, you might be developing a cosole presentation for 
debug and tests of your main web application. Either way, the only difference for using the EnTier 
library is how to start it. In a Console application, you will start application by calling 
```Scratch.Start();```. That's it!. 



EnTier Application Structure
============================

Following figure shows the structure of EnTier components stacked on each other. If you only extend EnTierControllerBase and 
don't add any Service or repositories, resulting structure would become simplified like the figure on right.



                                    [ Controller ]                              [ Controller ]
                              ____________|_____________                              |
                             |                          |                             |
                        [ Service ]                 [ Service ]                  [ Service ]
                             |__________________________|                             |
                                          |                                           |
                                          |                                           |
                                    [UnitOfWork]                                 [UnitOfWork]
                                          |                                           |
                           _______________|_______________                            |
                          |               |               |                           |
                   [ Repository ]  [ Repository ] ... [ Repository ]            [ Repository ]
                          |               |               |                           |
                          |_______________|_______________|                           |
                                          |                                           |
                                      [Context]                                   [Context]
                                           




Plugging In!
============

In an NTier application, There are two things you defenately need! A Mapper and A DI implementation 
(or IOC container). With your DI System configured, you can register your mapper on DI and then 
it will be used in whole project. So Mapper is determined by DI. You can register your IObjectMapper 
in DI Otherwinse, The EnTier's SimpleMapper will be used. This mapper as the name suggests, is simple 
and can't act very smart. ([More details on Simple mapper](#simple-mapper)).

How to Select your DI
----------------------

DI or IOC containers, can be selected at startup. Currently there are two Startup scenarios covered. One 
is ```Startup``` class in a MVC project. By default when you call ```services.AddEnTierServices()``` you are 
using MicrosoftDependencyInjection's Registerer. And calling ```app.UseEnTier()```, will set MicrosoftDependencyInjection's 
Resolver as DI-Resolver of the application.

The Other startup scenario is when you are writing a console application. And you start your application with ```Scratch``` 
class. There you can pass your implementations for IDIRegisterer and IDIResolver interfaces to ```Scratch.Start(.)``` overloads. 

Registering Mapper Or Any ServicesOn DI
--------------------------------

If you introduce your DI to EnTier application, you already have acces to it and you can register your services that way. 
But if you are planning to use EnTier's internal DI, you can access its registerer during the startup with IEnTierConfigurer. 
This will work fine either you introduce your DI or use the default. 



Simple Mapper
-------------

EnTier's Builtin Mapper

*   Is able to map fields with same names dispite their naming conventions (camelCase,PascalCase,snake_case,etc).
*   Can only map fields with directly castable types.
*   Can map Lists and Dictionaries.
