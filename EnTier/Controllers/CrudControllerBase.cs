using System;
using System.Collections.Generic;
using System.Net;
using EnTier.DataAccess.InMemory;
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
        protected IMapper Mapper { get; set; }
        protected IUnitOfWork UnitOfWork { get; set; }
        protected ICrudService<TDomain, TDomainId> Service { get; set; }

        protected ILogger Logger { get; } = EnTierLogging.GetInstance().Logger;

        protected IDataAccessRegulator<TDomain, TStorage> Regulator { get; } =
            new NullDataAccessRegulator<TDomain, TStorage>();

        public CrudControllerBase()
        {
            AcquirerDependencies();
        }

        public CrudControllerBase(IMapper mapper)
        {
            Mapper = mapper;

            AcquirerDependencies();
        }

        public CrudControllerBase(IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            Regulator = regulator;

            AcquirerDependencies();
        }

        public CrudControllerBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;

            AcquirerDependencies();
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork)
        {
            Mapper = mapper;

            UnitOfWork = unitOfWork;

            AcquirerDependencies();
        }

        public CrudControllerBase(IMapper mapper, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            Mapper = mapper;

            Regulator = regulator;

            AcquirerDependencies();
        }

        public CrudControllerBase(IUnitOfWork unitOfWork, IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            UnitOfWork = unitOfWork;

            Regulator = regulator;

            AcquirerDependencies();
        }

        public CrudControllerBase(IMapper mapper, IUnitOfWork unitOfWork,
            IDataAccessRegulator<TDomain, TStorage> regulator)
        {
            Mapper = mapper;

            UnitOfWork = unitOfWork;

            Regulator = regulator;

            AcquirerDependencies();
        }

        private void AcquirerDependencies()
        {
            if (Mapper == null)
            {
                Mapper = new EntierBuiltinMapper();
            }

            if (UnitOfWork == null)
            {
                UnitOfWork = new InMemoryUnitOfWork();
            }

            Service = AcquirerCrudService();
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
            return ErrorCheck(() => Ok(this.OnGetAll()));
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
    }
}