using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier.Repositories;

public abstract class DataBoundRepositoryBase:IDataBoundRepository
{

    public ILogger Logger { get; private set; } = new ConsoleLogger();


    public void SetLogger(ILogger logger)
    {
        Logger = logger;
    }

    public abstract Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress);

    public FilterRange GetDataRange(string headlessFieldAddress)
    {
        return GetDataRangeAsync(headlessFieldAddress).Result;
    }

    public abstract Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress);

    public List<string> GetExistingValues(string headlessFieldAddress)
    {
        return GetExistingValuesAsync(headlessFieldAddress).Result;
    }
}