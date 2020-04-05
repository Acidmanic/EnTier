


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