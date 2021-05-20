using System.Collections.Generic;
using EnTier.Mapper;
using EnTier.UnitOfWork;

namespace EnTier.Services
{
    public class CrudService<TDomain, TStorage, TId> : ICrudService<TDomain, TId>
        where TDomain : class, new()
        where TStorage : class, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CrudService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;

            _mapper = mapper;
        }

        public IEnumerable<TDomain> GetAll()
        {
            var storages = _unitOfWork.GetCrudRepository<TStorage, TId>().All();

            var domains = _mapper.Map<IEnumerable<TDomain>>(storages);

            return domains;
        }

        public TDomain GetById(TId id)
        {
            var storage = _unitOfWork.GetCrudRepository<TStorage, TId>().GetById(id);

            if (storage == null)
            {
                return null;
            }

            var domain = _mapper.Map<TDomain>(storage);

            return domain;
        }

        public TDomain Add(TDomain value)
        {
            var storage = _mapper.Map<TStorage>(value);

            storage = _unitOfWork.GetCrudRepository<TStorage, TId>().Add(storage);

            _unitOfWork.Complete();

            var domain = _mapper.Map<TDomain>(storage);

            return domain;
        }

        public TDomain Update(TDomain value)
        {
            var id = Utility.Reflection.GetPropertyReader<TDomain, TId>("Id").Invoke(value);

            var idReader = Utility.Reflection.GetPropertyReader<TStorage, TId>("Id");

            var foundValues = _unitOfWork.GetCrudRepository<TStorage, TId>()
                .Find(s => idReader(s).Equals(id));

            foreach (var found in foundValues)
            {
                _mapper.Map(value, found);

                return value;
            }

            return null;
        }

        public bool Remove(TDomain value)
        {
            var storage = _mapper.Map<TStorage>(value);

            return _unitOfWork.GetCrudRepository<TStorage, TId>().Remove(storage);
        }

        public bool RemoveById(TId id)
        {
            return _unitOfWork.GetCrudRepository<TStorage, TId>().Remove(id);
        }
    }
}