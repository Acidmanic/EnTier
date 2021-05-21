using EnTier.Mapper;
using EnTier.UnitOfWork;

namespace EnTier.Controllers
{
    public class CrudControllerBase<TDomain,TId>:CrudControllerBase<TDomain,TDomain,TDomain,TId>
    where TDomain:class,new()
    {
        public CrudControllerBase()
        {
        }

        public CrudControllerBase(IMapper mapper) : base(mapper)
        {
        }

        public CrudControllerBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
        }
    }
}