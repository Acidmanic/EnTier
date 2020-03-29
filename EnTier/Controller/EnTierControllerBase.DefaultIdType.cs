using EnTier.Configuration;
using EnTier.Plugging;
using EnTier.Service;
using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Utility;

namespace EnTier.Controllers
{
    public abstract class EnTierControllerBase
    <StorageModel, DomainModel, TransferModel>
    : EnTierControllerBase
    <StorageModel, DomainModel, TransferModel, long>
    where StorageModel : class
    {
        public EnTierControllerBase(IObjectMapper mapper) : base(mapper)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IService<DomainModel, long> service) : base(mapper, service)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider) : base(mapper, configurationProvider)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IService<DomainModel, long> service, IProvider<EnTierConfigurations> configurationProvider) : base(mapper, service, configurationProvider)
        {
        }
    }
}
