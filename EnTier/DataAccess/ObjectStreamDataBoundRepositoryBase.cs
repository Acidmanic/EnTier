using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Models;
using EnTier.Repositories;

namespace EnTier.DataAccess;


public class ObjectStreamDataBoundRepositoryBase<TStorage>:DataBoundRepositoryBase
{

    private IEnumerable<TStorage> _data;

    public ObjectStreamDataBoundRepositoryBase(IEnumerable<TStorage> data)
    {
        _data = data;
    }


    public override Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress)
    {
        return ObjectListRepositoryFilteringHelper
            .GetFilterRangeAsync(headlessFieldAddress, _data);
    }

    public override Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress)
    {
        return ObjectListRepositoryFilteringHelper
            .GetExistingValuesAsync(headlessFieldAddress, _data);
    }
    
}