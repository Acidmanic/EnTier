using System.Collections.Generic;
using EnTier.Mapper;
using EnTier.UnitOfWork;

namespace EnTier.Services
{
    public class CrudService<TDomain, TStorage, TDomainId, TStorageId> : ICrudService<TDomain, TDomainId>
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
            var storages = _unitOfWork.GetCrudRepository<TStorage, TDomainId>().All();

            var domains = _mapper.Map<IEnumerable<TDomain>>(storages);

            return domains;
        }

        public TDomain GetById(TDomainId id)
        {
            var storageId = _mapper.MapId<TStorageId>(id);

            var storage = _unitOfWork.GetCrudRepository<TStorage, TStorageId>().GetById(storageId);

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

            storage = _unitOfWork.GetCrudRepository<TStorage, TStorageId>().Add(storage);

            _unitOfWork.Complete();

            var domain = _mapper.Map<TDomain>(storage);

            return domain;
        }

        public TDomain Update(TDomain value)
        {
            var id = Utility.Reflection.GetPropertyReader<TDomain, TStorageId>("Id").Invoke(value);

            return Update(id, value);
        }

        public TDomain Update(TDomainId id, TDomain value)
        {
            var storageId = _mapper.MapId<TStorageId>(id);

            return Update(storageId, value);
        }

        private TDomain Update(TStorageId id, TDomain value)
        {
            var idReader = Utility.Reflection.GetPropertyReader<TStorage, TDomainId>("Id");

            var foundValues = _unitOfWork.GetCrudRepository<TStorage, TDomainId>()
                .Find(s => idReader(s).Equals(id));

            foreach (var found in foundValues)
            {
                _mapper.Map(value, found);

                _unitOfWork.Complete();

                return value;
            }

            return null;
        }

        public bool Remove(TDomain value)
        {
            var storage = _mapper.Map<TStorage>(value);

            var success = _unitOfWork.GetCrudRepository<TStorage, TStorageId>().Remove(storage);

            if (success)
            {
                _unitOfWork.Complete();
            }

            return success;
        }

        public bool RemoveById(TDomainId id)
        {
            var storageId = _mapper.MapId<TStorageId>(id);

            var success = _unitOfWork.GetCrudRepository<TStorage, TStorageId>().Remove(storageId);
            
            if (success)
            {
                _unitOfWork.Complete();
            }
            return success;
        }
    }
}