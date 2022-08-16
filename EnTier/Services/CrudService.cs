using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.TypeCenter;
using EnTier.Exceptions;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.UnitOfWork;
using EnTier.Utility;

namespace EnTier.Services
{
    public class CrudService<TDomain, TStorage, TDomainId, TStorageId> : ICrudService<TDomain, TDomainId>
        where TDomain : class, new()
        where TStorage : class, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDataAccessRegulator<TDomain, TStorage> _regulator;
        private readonly AccessNode _storageIdLeaf = IdHelper.GetIdNode<TStorage, TStorageId>();
        private readonly AccessNode _domainIdLeaf = IdHelper.GetIdNode<TDomain, TDomainId>();
        
        public CrudService(IUnitOfWork unitOfWork, IMapper mapper) : this(unitOfWork, mapper,
            new NullDataAccessRegulator<TDomain, TStorage>())
        {
        }

        public CrudService(IUnitOfWork unitOfWork, IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _unitOfWork = unitOfWork;

            _mapper = mapper;

            _regulator = regulator;
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

        private TStorage Regulate(TDomain value)
        {
            TStorage regulatedStorage = default;

            if (_regulator is NullDataAccessRegulator)
            {
                regulatedStorage = _mapper.Map<TStorage>(value);
            }
            else
            {
                var regulationResult = _regulator.Regulate(value);

                if (regulationResult.Status != RegulationStatus.UnAcceptable)
                {
                    regulatedStorage = regulationResult.Storage;
                }
                else
                {
                    throw new UnAcceptableModelException();
                }
            }

            return regulatedStorage;
        }
        
        public TDomain Add(TDomain value)
        {
            var storage = Regulate(value);

             storage =  _unitOfWork.GetCrudRepository<TStorage, TStorageId>().Add(storage);

            _unitOfWork.Complete();

            var domain = _mapper.Map<TDomain>(storage);

            return domain;
        }

        public TDomain Update(TDomain value)
        {
            if (_domainIdLeaf.Evaluator.Read(value) is TDomainId id )
            {
                return Update(id, value);    
            }
            // Not Successful
            return null;
        }

        public TDomain Update(TDomainId id, TDomain value)
        {
            var storageId = _mapper.MapId<TStorageId>(id);

            return Update(storageId, value);
        }

        private TDomain Update(TStorageId id, TDomain value)
        {
            Regulate(value);

            var repo = _unitOfWork.GetCrudRepository<TStorage, TDomainId>();

            var found = repo.Find(s => _storageIdLeaf.Evaluator.Read(s).Equals(id))
                .FirstOrDefault();

            if(found != null)
            {
                
                var storageUpdate =_mapper.Map<TStorage>(value);

                repo.Set(storageUpdate);

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