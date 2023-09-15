using Meadow.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier.DataAccess.Meadow.Extensions;

public static class MeadowRequestExceptionLoggingExtensions
{




    public static void LogIfFailed(this MeadowRequest response, ILogger logger = null)
    {
        logger ??= new ConsoleLogger().Shorten().EnableAll();
        
        if (response.Failed)
        {
            logger.LogError(response.FailureException, "Meadow Request Failed:");
            logger.LogError("{Exception}",response.FailureException);
        }
    }
}