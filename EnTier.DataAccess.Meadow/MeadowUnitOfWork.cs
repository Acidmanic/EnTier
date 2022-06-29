using EnTier.Repositories;
using EnTier.UnitOfWork;
using Meadow.Configuration;
using Meadow.Scaffolding.Contracts;

namespace EnTier.DataAccess.Meadow
{
    public class MeadowUnitOfWork:UnitOfWorkBase
    {
        public MeadowUnitOfWork(IMeadowConfigurationProvider configurationProvider)
        {
            ConfigurationProvider = configurationProvider;
        }

        private class WrapperProvider : IMeadowConfigurationProvider
        {
            private readonly MeadowConfiguration _config;

            public WrapperProvider(MeadowConfiguration config)
            {
                _config = config;
            }

            public MeadowConfiguration GetConfigurations()
            {
                return _config;
            }
        }
        
        public MeadowUnitOfWork(MeadowConfiguration configuration)
        {
            ConfigurationProvider = new WrapperProvider(configuration);
        }

        private IMeadowConfigurationProvider ConfigurationProvider { get; }
        
        protected override ICrudRepository<TStorage, TId> CreateDefaultCrudRepository<TStorage, TId>()
        {
            return new MeadowCrudRepository<TStorage, TId>(ConfigurationProvider.GetConfigurations());
        }

        public override void Complete()
        {
            // Cant be done truly as unit of work, unless 1- EnTIer becomes async. 2- Meadow Engine gets a batch perform  Requests method  
        }

        public override void Dispose()
        {
            //
        }
    }
}