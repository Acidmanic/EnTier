using System;
using EnTier.DataAccess.InMemory;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;

namespace EnTier.EnTierEssentials
{
    internal class DefaultEssentialProvider<TDomain,TStorage, TDomainId,TStorageId> :
        ServiceEssentialsProvider<TDomain,TStorage, TDomainId,TStorageId> where TDomain : class, new() where TStorage : class, new()
    {
        
    
        public DefaultEssentialProvider(IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IDataAccessRegulator<TDomain, TStorage> regulator,
            ICrudService<TDomain,TDomainId> service) : base(mapper, unitOfWork, regulator)
        {
            Service = Acquire(service, () => new CrudService<TDomain, TStorage, TDomainId, TStorageId>());
            
            if (Service is CrudService<TDomain, TStorage, TDomainId, TStorageId> initee)
            {
                initee.InitializeEssentials(UnitOfWork, Mapper, DataAccessRegulator);    
            }
        }
        public ICrudService<TDomain, TDomainId> Service { get; }

    }
}