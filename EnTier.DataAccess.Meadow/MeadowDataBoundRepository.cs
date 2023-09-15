using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnTier.DataAccess.Meadow.Extensions;
using EnTier.DataAccess.Meadow.GenericFilteringRequests;
using EnTier.DataAccess.Meadow.GenericFilteringRequests.Models;
using EnTier.Models;
using EnTier.Repositories;
using Meadow;
using Meadow.Configuration;
using Meadow.Requests;
using Microsoft.Extensions.Logging;

namespace EnTier.DataAccess.Meadow;

public class MeadowDataBoundRepository<TStorage>:DataBoundRepositoryBase
{
    public MeadowDataBoundRepository(MeadowConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected MeadowConfiguration Configuration { get; }
    
    
    protected MeadowEngine GetEngine()
    {
        var engine = new MeadowEngine(Configuration);

        MeadowEngine.UseLogger(Logger);

        return engine;
    }

    public override async Task<FilterRange> GetDataRangeAsync(string headlessFieldAddress)
    {
        var request = new RangeRequest<TStorage>(headlessFieldAddress);
            
        var engine = GetEngine();

        var response = await engine.PerformRequestAsync(request);
            
        response.LogIfFailed(Logger);

        var readRange = response.FromStorage.FirstOrDefault() ?? new FieldRange();
            
        return readRange.ToFilterRange();
            
    }

    public override async Task<List<string>> GetExistingValuesAsync(string headlessFieldAddress)
    {
        var request = new ExistingValuesRequest<TStorage>(headlessFieldAddress);
            
        var engine = GetEngine();

        var response = await engine.PerformRequestAsync(request);
            
        response.LogIfFailed(Logger);

        return response.FromStorage.Select(o => o.ToString()).ToList();
    }
}