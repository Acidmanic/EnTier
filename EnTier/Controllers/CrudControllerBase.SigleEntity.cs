using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.UnitOfWork;

namespace EnTier.Controllers
{
    public class CrudControllerBase<TDomain,TId>:CrudControllerBase<TDomain,TDomain,TDomain,TId>
    where TDomain:class,new()
    {
        public CrudControllerBase(EnTierEssence essence) : base(essence)
        {
        }
    }
}