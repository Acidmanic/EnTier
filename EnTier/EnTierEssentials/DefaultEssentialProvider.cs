using System;
using EnTier.DataAccess.InMemory;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;

namespace EnTier.EnTierEssentials
{
    internal class DefaultEssentialProvider<TDomain,TStorage, TDomainId,TStorageId>:IEnTierEssentialsProvider<TDomain,TStorage, TDomainId,TStorageId> where TDomain : class, new() where TStorage : class, new()
    {
        

        public DefaultEssentialProvider(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IDataAccessRegulator<TDomain,TStorage> regulator,
            ICrudService<TDomain,TDomainId> service)
        {

            Mapper = Acquire(mapper, () => new EntierBuiltinMapper());
            UnitOfWork = Acquire(unitOfWork, () => new InMemoryUnitOfWork());
            DataAccessRegulator = Acquire(regulator, () => new NullDataAccessRegulator<TDomain, TStorage>());
            
            
            Service = Acquire(service, () => new CrudService<TDomain, TStorage, TDomainId, TStorageId>());

            if (Service is CrudService<TDomain, TStorage, TDomainId, TStorageId> initee)
            {
                initee.InitializeEssentials(UnitOfWork, Mapper, DataAccessRegulator);    
            }
            
            

        }
        
        
        
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IDataAccessRegulator<TDomain, TStorage> DataAccessRegulator { get; }
        public ICrudService<TDomain, TDomainId> Service { get; }


        private T Acquire<T>(T provided, Func<T> defaultFactory)
        {
            if (provided != null)
            {
                return provided;
            }

            var resolved = EnTierEssence.Resolver.TryResolve<T>();

            if (resolved)
            {
                if (resolved.Value != null)
                {
                    return resolved;
                }
            }

            return defaultFactory();
        }
    }
}