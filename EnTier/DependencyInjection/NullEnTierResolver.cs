using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier.DependencyInjection
{
    internal class NullEnTierResolver : EnTierResolver
    {
        private class NullResolverFacade : IResolverFacade
        {
            public NullResolverFacade()
            {
            }

            public object Resolve(Type type)
            {
                new ConsoleLogger()
                    .LogError("No Resolver has been introduced to EnTier. " +
                              "\n\t\tPlease Use EnTierEssence " +
                              "to introduce your resolver.");

                return null;
            }
        }

        public NullEnTierResolver() : base(new NullResolverFacade())
        {
        }
    }
}