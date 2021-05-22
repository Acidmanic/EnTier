







Manipulate Crud Service
--------

Override Acquirer CrudService


Manipulate Mapper
----------

Through The Injection

Declare Unit Of Work
------------

Each application will has its own unit of work and repositories, 
isolating the data access layer. If you dont define any unit of work, 
EnTier will create an InMemory UnitOfWork which will produce InMemory 
Crud Repositories. But in real case scenario you will create your own 
UnitOfWork class and inject it into CrudControllers. For this to work 
you need to implement ```IUnitOfWork``` in it, so the CrudControllers 
use the same data access layer as your main application. You also need 
to deliver this unitOfWork instance through your DI, so you need to 
override one of the constructors in the CrudControllerBase which takes
 IUnitOfWork argument.



Directly Supported DI
================


So far, Only dotnet core's builtin DI system is supported. 


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

Notes
=========

 * Builtin Mapper is no good for production, 
 its just for library to work out of box to ease the 
 process of familiarizing and first usages. Using 
 Automapper is Highly recommended.  
 