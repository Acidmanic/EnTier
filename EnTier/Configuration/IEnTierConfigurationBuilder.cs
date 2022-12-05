using System;
using EnTier.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnTier.Configuration
{
    public interface IEnTierConfigurationBuilder
    {
        
        IEnTierConfigurationBuilder SetLogger(ILogger logger);

        IEnTierConfigurationBuilder UseResolver(IResolverFacade resolver);
        
        IEnTierConfigurationBuilder UseResolver(Func<Type,object> resolver);

        IEnTierConfigurationBuilder Clear();

    }
}