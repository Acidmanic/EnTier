using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Models;

namespace EnTier.Contracts;

public interface IFilterInformationService
{
    Task<List<FilterProfile>> GetFilterProfileAsync<TStorage, TId>(bool fullTree) where TStorage : class, new();
    
    List<FilterProfile> GetFilterProfile<TStorage, TId>(bool fullTree) where TStorage : class, new();
    
}