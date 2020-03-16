



using Utility;

namespace Configuration{


    public class DefaulConfigurationsProvider : IProvider<EnTierConfigurations>
    {
        public EnTierConfigurations Create()
        {
            return new EnTierConfigurations{
                ExposesExceptions = false
            };
        }
    }
}