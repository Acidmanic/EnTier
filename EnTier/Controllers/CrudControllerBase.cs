using System;
using System.Collections.Generic;
using System.Net;
using Acidmanic.Utilities.Reflection;
using EnTier.DataAccess.InMemory;
using EnTier.EnTierEssentials;
using EnTier.Extensions;
using EnTier.Logging;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EnTier.Controllers
{
    public abstract class
        CrudControllerBase<TTransfer, TDomain, TStorage, TTransferId, TDomainId, TStorageId> : ControllerBase
        where TTransfer : class, new()
        where TDomain : class, new()
        where TStorage : class, new()
    {

        private readonly IEnTierEssentialsProvider<TDomain, TStorage, TDomainId, TStorageId> _essentialsProvider;

        protected IMapper Mapper => _essentialsProvider.Mapper;
        protected IUnitOfWork UnitOfWork => _essentialsProvider.UnitOfWork;
        protected ICrudService<TDomain, TDomainId> Service => _essentialsProvider.Service;

        protected ILogger Logger { get; } = EnTierLogging.GetInstance().Logger;

        protected virtual bool AutoWrap { get; } = true;
        
        protected EnumerableDynamicWrapper<TTransfer> AutoWrapper = new EnumerableDynamicWrapper<TTransfer>();
        protected IDataAccessRegulator<TDomain, TStorage> Regulator { get; } =
            new NullDataAccessRegulator<TDomain, TStorage>();

        public CrudControllerBase()
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                null,
                null,
                null,
                null
            );
        }

        public CrudControllerBase(IMapper mapper)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                mapper,
                null,
                null,
                null
                );
        }

        public CrudControllerBase(IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                null,
                null,
                regulator,
                null
            );
        }

        public CrudControllerBase(IUnitOfWork unitOfWork)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                null,
                unitOfWork,
                null,
                null
            );
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                mapper,
                unitOfWork,
                null,
                null
            );
        }

        public CrudControllerBase(IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                mapper,
                null,
                regulator,
                null
            );
        }

        public CrudControllerBase(IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                null,
                unitOfWork,
                regulator,
                null
            );
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork,
            IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            _essentialsProvider = new DefaultEssentialProvider<TDomain, TStorage, TDomainId, TStorageId>(
                mapper,
                unitOfWork,
                null,
                null
            );
        }
        
        protected virtual ICrudService<TDomain, TDomainId> AcquirerCrudService()
        {
            return new CrudService<TDomain, TStorage, TDomainId, TStorageId>(UnitOfWork, Mapper, Regulator);
        }


        private IActionResult ErrorCheck(Func<IActionResult> code)
        {
            try
            {
                return code.Invoke();
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll()
        {
            if (AutoWrap)
            {
                return ErrorCheck(() =>
                {
                    var data = OnGetAll();
                    
                    var wrapped = AutoWrapper.Wrap(data);
                    
                    return Ok(wrapped);
                }); 
            }
            return ErrorCheck(() =>
            {
                return Ok(this.OnGetAll());
            });
        }


        protected virtual IEnumerable<TTransfer> OnGetAll()
        {
            var allDomainObjects = Service.GetAll();

            var allTransferObjects = Mapper.Map<List<TTransfer>>(allDomainObjects);

            return allTransferObjects;
        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(TTransferId id)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnGetById(id);

                if (transfer == null)
                {
                    return NotFound();
                }

                return Ok(transfer);
            });
        }

        protected virtual TTransfer OnGetById(TTransferId id)
        {
            var domainId = Mapper.MapId<TDomainId>(id);

            var domain = Service.GetById(domainId);

            var transfer = domain == null ? null : Mapper.Map<TTransfer>(domain);

            return transfer;
        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnCreateNew(value);

                return StatusCode((int) HttpStatusCode.Created, transfer);
            });
        }

        protected virtual TTransfer OnCreateNew(TTransfer value)
        {
            var domain = Mapper.Map<TDomain>(value);

            domain = Service.Add(domain);

            var transfer = domain == null ? null : Mapper.Map<TTransfer>(domain);

            return transfer;
        }

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TTransferId id, TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnUpdate(id, value);

                return Ok(transfer);
            });
        }

        protected virtual TTransfer OnUpdate(TTransferId id, TTransfer value)
        {
            var domain = Mapper.Map<TDomain>(value);

            domain = Service.Update(domain);

            var transfer = domain == null ? null : Mapper.Map<TTransfer>(domain);

            return transfer;
        }

        [HttpPut]
        [Route("{id}")]
        public virtual IActionResult Update(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnUpdate(value);

                return Ok(value);
            });
        }

        protected virtual TTransfer OnUpdate(TTransfer value)
        {
            var domain = Mapper.Map<TDomain>(value);

            domain = Service.Update(domain);

            var transfer = domain == null ? null : Mapper.Map<TTransfer>(domain);

            return transfer;
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual IActionResult DeleteById(TTransferId id)
        {
            return ErrorCheck(() =>
            {
                var success = OnDeleteById(id);

                if (!success)
                {
                    return NotFound();
                }

                return Ok();
            });
        }

        protected virtual bool OnDeleteById(TTransferId id)
        {
            var domainId = Mapper.MapId<TDomainId>(id);

            var success = Service.RemoveById(domainId);

            return success;
        }

        [HttpDelete]
        [Route("")]
        public virtual IActionResult Delete(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var success = OnDelete(value);

                if (!success)
                {
                    return NotFound();
                }

                return Ok();
            });
        }


        protected virtual bool OnDelete(TTransfer value)
        {
            var domain = Mapper.Map<TDomain>(value);

            var success = Service.Remove(domain);

            return success;
        }

        protected IActionResult WrapCollection<T>(IEnumerable<T> data)
        {
            return WrapCollection(data, HttpStatusCode.OK);
        }
        protected IActionResult WrapCollection<T>(IEnumerable<T> data, string name )
        {
            return WrapCollection(data, HttpStatusCode.OK, name);
        }

        protected IActionResult WrapCollection<T>(IEnumerable<T> data, HttpStatusCode status)
        {
            return WrapCollection(data, HttpStatusCode.OK, typeof(T).Name.Plural());
        }

        protected IActionResult WrapCollection<T>(IEnumerable<T> data, HttpStatusCode status, string name)
        {
            var wrapper = new EnumerableDynamicWrapper<T>(name);

            var wrappedObject = wrapper.Wrap(data);
            
            return StatusCode((int) status, wrappedObject);
        }
    }
}