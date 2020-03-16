


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Utility;

namespace Configuration{





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