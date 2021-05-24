using EnTier.Mapper;
using EnTier.UnitOfWork;

namespace EnTier.Controllers
{
    public class CrudControllerBase<TTransfer, TDomain, TStorage,TId>:
        CrudControllerBase<TTransfer, TDomain, TStorage,TId,TId,TId>
        where TTransfer : class, new()
        where TDomain : class, new()
        where TStorage : class, new()
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