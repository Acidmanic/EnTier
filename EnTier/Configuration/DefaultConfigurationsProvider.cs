



using EnTier.Utility;

namespace EnTier.Configuration
{


    public class DefaultConfigurationsProvider : IProvider<EnTierConfigurations>
    {
        public EnTierConfigurations Create()
        {
            return new EnTierConfigurations{
                ExposesExceptions = false
            };
        }
    }
}