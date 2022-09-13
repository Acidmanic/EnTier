using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;

namespace EnTier.EnTierEssentials
{
    public interface IEnTierEssentialsProvider<TDomain,TStorage, in TDomainId,TStorageId> where TDomain : class, new()
    {
        
        
        
        public IMapper Mapper { get; }
        
        public IUnitOfWork UnitOfWork { get; }
        
        public IDataAccessRegulator<TDomain,TStorage> DataAccessRegulator { get; }
        
        public ICrudService<TDomain,TDomainId> Service { get; }
        
        
    }
}