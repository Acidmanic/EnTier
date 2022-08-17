using Microsoft.Extensions.Logging;

namespace EnTier.Extensions
{
    public static class LoggerExtensions
    {
        
        public static ILogger UseLoggerForEnTier(this ILogger logger)
        {
            EnTierLogging.GetInstance().Set(logger);

            return logger;
        }
    }
}