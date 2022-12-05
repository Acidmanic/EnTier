using Meadow.Configuration;
using Meadow.Scaffolding.Contracts;

namespace EnTier.DataAccess.Meadow
{
    internal class ByInstanceMeadowConfigurationProvider : IMeadowConfigurationProvider
    {
        private readonly MeadowConfiguration _config;

        public ByInstanceMeadowConfigurationProvider(MeadowConfiguration config)
        {
            _config = config;
        }

        public MeadowConfiguration GetConfigurations()
        {
            return _config;
        }
    }
}