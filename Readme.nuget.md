Easy NTier (EnTier) Library  v2.0.0
===========================

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
 ```IUnitOfWork``` interface or to have crud operations already implemented, you can extend 
 or wrap ```UnitOfWork``` class shipped with the package to do so.
 * For Repositories, it's the same as Services; you can write them from scratch, implement 
 ```ICrudRepository``` or extend (or wrap) ```CrudRepository```.
 
 
 More Details
 ==============
 
 Since NuGet can not display md files larger than 8000 bytes, Refer to [EiTier's GitHub page](https://github.com/Acidmanic/EnTier) 
  for more details, examples and descriptions.
  
  
```text
  Thanks and regards.
  Mani
```
