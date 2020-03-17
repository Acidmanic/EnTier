



using Utility;

namespace Configuration{


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