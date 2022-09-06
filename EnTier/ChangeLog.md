
2.3.1
-----


 * Mapper, Service, Regulator  and UnitOfWork properties of 
 CrudControllerBase became protected (instead of private),
  so they are accessible in driven classes.
  
  
 2.3.2
 -----
 
  * Add overridable crud methods before error checking for easier driving methods.
    
  2.4.2
  -----
  
  * Separate Repository Creation from Indexing it in InMemoryRepositoryBase, 
    Fixes the issue of un-indexed custom repositories. 
    
    
  2.4.3
  ------
   
   * Fixed Misleading Custom Repository registration
    
  2.4.4
  -----
  
   * Added ```UseFixture<TFixture>``` also to IServiceProvider.
   * Fixed StorageRequest<> regarding meadow updates
    
  2.5.0
  -----
  
   * Updated Reflection to b7
   * Added Set Method to repository
   
   2.5.1
   -----
   * Updated Reflection to b8
   * Fix Set and Update issues
   * Clarify and update Id-field detection
   
  2.5.3
  -----
   * Add Logging
   * Add Extension method to ILoggers so they can be set for EnTier logging 

  2.5.6
  ------
  
   * Update Reflection module
   * Add Logging Adapters for EnTier
   * Add FixtureManager to help using fixtures in non-web applications
   * Fix IdGenerator's casting issue
   * Limit InMemory and JsonFile crud repositories id-generation to only AutoValues Ids
   
   
 2.5.7
 ------
   * Update Meadow to 1.1.0
   * Allow access to Main Properties for driven classes of CrudService    
   
   
 2.5.8
 -----
   * Controllers will be (by default) able to auto-wrap returning collections on Get-All
   
 2.5.9
 -----
   * Added Prepopulation Module, help seeding in production (vs fixtures which are aimed for tests)
   * Updated Meadow to 1.1.2
 
2.5.10
-----
  * Replace IdHelper with TypeIdentity
  * Replace CollectionDtoWrapper with EnumerableDynamicWrapper
  * Make Prepopulation Conform with ILogger