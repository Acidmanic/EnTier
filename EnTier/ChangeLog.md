



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
   
   