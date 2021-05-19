


using Microsoft.Extensions.Configuration;
using EnTier.Utility;

namespace EnTier.Configuration
{





    public class MSExtensionsConfigurationsProvider : IProvider<EnTierConfigurations>
    {

        private readonly IConfiguration _configuration;

        public MSExtensionsConfigurationsProvider(IConfiguration configuration){
            _configuration = configuration;
        }
 
        public EnTierConfigurations Create()
        {
            try
            {
                return _configuration.GetSection(nameof(EnTierConfigurations))
                    .Get<EnTierConfigurations>();
            }
            catch (System.Exception)
            {            }
            return new EnTierConfigurations();
        }
    }
}