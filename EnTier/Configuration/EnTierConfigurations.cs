using EnTier.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnTier.Configuration
{
    internal class EnTierConfigurations
    {
        
        public ILogger Logger { get; set; }
        
        public IResolverFacade Resolver { get; set; }
        
        
    }
}