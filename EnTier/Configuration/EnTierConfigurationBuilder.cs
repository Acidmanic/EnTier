using System;
using EnTier.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnTier.Configuration
{
    internal class EnTierConfigurationBuilder : IEnTierConfigurationBuilder
    {
        private EnTierConfigurations _configurations;


        public EnTierConfigurationBuilder()
        {
            Clear();
        }

        public IEnTierConfigurationBuilder Clear()
        {
            _configurations = new EnTierConfigurations
            {
                Logger = NullLogger.Instance,
                Resolver = new NullResolver()
            };

            return this;
        }

        public IEnTierConfigurationBuilder SetLogger(ILogger logger)
        {
            _configurations.Logger = logger;

            return this;
        }

        public IEnTierConfigurationBuilder UseResolver(IResolverFacade resolver)
        {
            _configurations.Resolver = resolver;

            return this;
        }

        public IEnTierConfigurationBuilder UseResolver(Func<Type, object> resolver)
        {
            _configurations.Resolver = new ResolverAdapter(resolver);

            return this;
        }

        public EnTierConfigurations Build()
        {
            return _configurations;
        }
    }
}