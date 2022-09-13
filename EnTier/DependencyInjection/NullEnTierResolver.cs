using System;
using Microsoft.Extensions.Logging;

namespace EnTier.DependencyInjection
{
    internal class NullEnTierResolver:EnTierResolver
    {
        private class NullResolverFacade:IResolverFacade
        {
            private readonly ILogger _logger;

            public NullResolverFacade(ILogger logger)
            {
                _logger = logger;
            }

            public object Resolve(Type type)
            {
                _logger.LogError("No Resolver has been introduced to EnTier. " +
                                 "\n\t\tPlease Use EnTierEssence " +
                                 "to introduce your resolver.");

                return null;
            }
        }

        public NullEnTierResolver(ILogger logger) : base(new NullResolverFacade(logger))
        {
            
        }
        
    }
}