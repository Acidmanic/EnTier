using System;
using System.Collections.Generic;
using System.Net;
using EnTier.DataAccess.InMemory;
using EnTier.Mapper;
using EnTier.Services;
using EnTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Controllers
{
    public abstract class
        CrudControllerBase<TTransfer, TDomain, TStorage, TTransferId, TDomainId, TStorageId> : ControllerBase
        where TTransfer : class, new()
        where TDomain : class, new()
        where TStorage : class, new()
    {
        private IMapper Mapper { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        private ICrudService<TDomain, TDomainId> Service { get; set; }


        public CrudControllerBase()
        {
            AcquirerDependencies();
        }

        public CrudControllerBase(IMapper mapper)
        {
            Mapper = mapper;

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
            return new CrudService<TDomain, TStorage, TDomainId, TStorageId>(UnitOfWork, Mapper);
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
            return ErrorCheck(() =>
            {
                var allDomainObjects = Service.GetAll();

                var allTransferObjects = Mapper.Map<List<TTransfer>>(allDomainObjects);

                return Ok(allTransferObjects);
            });
        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(TTransferId id)
        {
            return ErrorCheck(() =>
            {
                var domainId = Mapper.MapId<TDomainId>(id);

                var domain = Service.GetById(domainId);

                if (domain == null)
                {
                    return NotFound();
                }

                var transfer = Mapper.Map<TTransfer>(domain);

                return Ok(transfer);
            });
        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var domain = Mapper.Map<TDomain>(value);

                domain = Service.Add(domain);

                var transfer = Mapper.Map<TTransfer>(domain);

                return StatusCode((int) HttpStatusCode.Created, transfer);
            });
        }


        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TTransferId id, TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var domain = Mapper.Map<TDomain>(value);

                domain = Service.Update(domain);

                var transfer = Mapper.Map<TTransfer>(domain);

                return Ok(transfer);
            });
        }

        [HttpPut]
        [Route("{id}")]
        public virtual IActionResult Update(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var domain = Mapper.Map<TDomain>(value);

                domain = Service.Update(domain);

                var transfer = Mapper.Map<TTransfer>(domain);

                return Ok(value);
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual IActionResult DeleteById(TTransferId id)
        {
            return ErrorCheck(() =>
            {
                var domainId = Mapper.MapId<TDomainId>(id);

                var success = Service.RemoveById(domainId);

                if (!success)
                {
                    return NotFound();
                }

                return Ok();
            });
        }

        [HttpDelete]
        [Route("")]
        public virtual IActionResult Delete(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var domain = Mapper.Map<TDomain>(value);

                var success = Service.Remove(domain);

                if (!success)
                {
                    return NotFound();
                }

                return Ok();
            });
        }
    }
}