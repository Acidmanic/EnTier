







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



Notes
=========

 * Builtin Mapper is no good for production, 
 its just for library to work out of box to ease the 
 process of familiarizing and first usages. Using 
 Automapper is Highly recommended.  
 