


What Is A Channel
=================

Every API Endpoint is an entry wich gives the user access 
to a resouce. For example this resource can be considered 
a data in database, and the access can be a CRUD operation. 
performing such a task in a 3-Tier/Mvc like manner, will requier 
a Controller, a Service, and a repository. in ENTier libarary, 
repositories are not delivered directly to the service. It will 
be done via a UnitOfWork. Also Repositories can work with any 
Context. In case of using a database, it will get a DatabaseContext. 
Therefore for each access we would go through 5 objects sofar:

Controller
 |
Service
 |
UnitOfWork \
 |          |<--- Context
Repository /

Such composition, Forms a Channel. A Channel is a Combination of every 
object aming to access a specific resource from higher layer to lower 
layer.


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