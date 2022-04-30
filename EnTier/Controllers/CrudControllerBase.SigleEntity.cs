using EnTier.Mapper;
using EnTier.Regulation;
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

        public CrudControllerBase(IDataAccessRegulator<TDomain, TDomain> regulator) : base(regulator)
        {
        }

        public CrudControllerBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
        }

        public CrudControllerBase(IMapper mapper, IDataAccessRegulator<TDomain, TDomain> regulator) : base(mapper, regulator)
        {
        }

        public CrudControllerBase(IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TDomain> regulator) : base(unitOfWork, regulator)
        {
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TDomain> regulator) : base(mapper, unitOfWork, regulator)
        {
        }
    }
}