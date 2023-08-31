using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Repositories;

public class NullDataBoundRepository : DataBoundRepositoryBase
{
    private static IDataBoundRepository _instance = null;

    public static IDataBoundRepository Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NullDataBoundRepository();
            }

            return _instance;
        }
    }

    private NullDataBoundRepository()
    {
    }

    public override Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress)
    {
        Logger.LogError("Current Data Access system does not support filtering");

        return Task.FromResult(new FilterRange());
    }

    public override Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress)
    {
        Logger.LogError("Current Data Access system does not support filtering");

        return Task.FromResult(new List<string>());
    }
}