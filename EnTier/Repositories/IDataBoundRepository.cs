using System.Collections.Generic;
using System.Threading.Tasks;
using EnTier.Models;
using Microsoft.Extensions.Logging;

namespace EnTier.Repositories;

public interface IDataBoundRepository
{
    /// <summary>
    /// Sets the internal repository logger
    /// </summary>
    /// <param name="logger"></param>
    void SetLogger(ILogger logger);
    Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress);

    FilterRange GetDataRange(string headlessFieldAddress);

    Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress);

    List<string> GetExistingValues(string headlessFieldAddress);
}