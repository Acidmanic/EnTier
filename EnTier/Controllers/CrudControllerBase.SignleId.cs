using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.UnitOfWork;

namespace EnTier.Controllers
{
    public class CrudControllerBase<TTransfer, TDomain, TStorage,TId>:
        CrudControllerBase<TTransfer, TDomain, TStorage,TId,TId,TId>
        where TTransfer : class, new()
        where TDomain : class, new()
        where TStorage : class, new()
    {
        public CrudControllerBase(EnTierEssence essence) : base(essence)
        {
        }
    }
}