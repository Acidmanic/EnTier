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
        public CrudControllerBase()
        {
        }

        public CrudControllerBase(IMapper mapper) : base(mapper)
        {
        }

        public CrudControllerBase(IDataAccessRegulator<TDomain, TStorage> regulator) : base(regulator)
        {
        }

        public CrudControllerBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
        }

        public CrudControllerBase(IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator) : base(mapper, regulator)
        {
        }

        public CrudControllerBase(IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TStorage> regulator) : base(unitOfWork, regulator)
        {
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TStorage> regulator) : base(mapper, unitOfWork, regulator)
        {
        }
    }
}