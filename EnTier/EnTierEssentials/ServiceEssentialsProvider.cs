using System;
using EnTier.DataAccess.InMemory;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;

namespace EnTier.EnTierEssentials
{
    internal class ServiceEssentialsProvider<TDomain,TStorage, TDomainId,TStorageId> where TDomain : class, new() where TStorage : class, new()
    {
        

        public ServiceEssentialsProvider(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IDataAccessRegulator<TDomain,TStorage> regulator)
        {

            Mapper = Acquire(mapper, () => new EntierBuiltinMapper());
            UnitOfWork = Acquire(unitOfWork, () => new InMemoryUnitOfWork());
            DataAccessRegulator = Acquire(regulator, () => new NullDataAccessRegulator<TDomain, TStorage>());
            
        }
        
        
        
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IDataAccessRegulator<TDomain, TStorage> DataAccessRegulator { get; }


        protected T Acquire<T>(T provided, Func<T> defaultFactory)
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