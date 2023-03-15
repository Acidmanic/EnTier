using EnTier.Repositories;
using EnTier.UnitOfWork;
using Meadow.Configuration;
using Meadow.Contracts;

namespace EnTier.DataAccess.Meadow
{
    public class MeadowUnitOfWork : UnitOfWorkBase
    {
        public MeadowUnitOfWork(EnTierEssence essence, IMeadowConfigurationProvider configurationProvider) :
            base(essence)
        {
            ConfigurationProvider = configurationProvider;
        }


        public MeadowUnitOfWork(EnTierEssence essence, MeadowConfiguration configuration) : this(essence,
            new ByInstanceMeadowConfigurationProvider(configuration))
        {
        }

        private IMeadowConfigurationProvider ConfigurationProvider { get; }

        public override IEventStreamRepository<TEvent, TEventId, TStreamId> GetStreamRepository<TEvent, TEventId,
            TStreamId>()
        {
            return new MeadowEventStreamRepository<TEvent, TEventId, TStreamId>(ConfigurationProvider, Essence);
        }

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