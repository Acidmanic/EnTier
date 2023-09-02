using System;
using System.Collections.Generic;
using System.Net;
using Acidmanic.Utilities.Reflection;
using EnTier.AutoWrap;
using EnTier.Extensions;
using EnTier.Mapper;
using EnTier.Models;
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
        private readonly EnTierEssence _essence;

        protected IMapper Mapper { get; private set; }
        protected IUnitOfWork UnitOfWork { get; private set; }
        protected ICrudService<TDomain, TDomainId> Service { get; private set; }

        protected ILogger Logger { get; private set; }

        protected readonly EnTierAutoWrapper<TTransfer> AutoWrapper;

        protected IDataAccessRegulator<TDomain, TStorage> Regulator { get; private set; }

        public CrudControllerBase(EnTierEssence essence)
        {
            _essence = essence;

            Mapper = essence.Mapper;
            UnitOfWork = essence.UnitOfWork;
            Service = essence.ResolveOrDefault<ICrudService<TDomain, TDomainId>>(() =>
                new CrudService<TDomain, TStorage, TDomainId, TStorageId>(essence));
            Logger = essence.Logger;
            Regulator = essence.Regulator<TDomain, TStorage>();
            AutoWrapper = new EnTierAutoWrapper<TTransfer>(this);
        }


        private IActionResult ErrorCheck(Func<IActionResult> code)
        {
            try
            {
                return code.Invoke();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll()
        {
            return ErrorCheck(() =>
            {
                var data = OnGetAll();

                return Ok(data);
            });
        }


        protected virtual Chunk<TTransfer> OnGetAll()
        {
            var filter = HttpContext.GetFilter<TStorage>();

            var pagination = HttpContext.GetPagination();

            var domainChunk = Service.GetAllAsync(
                pagination.Offset,
                pagination.Size,
                pagination.SearchId,
                filter).Result;

            var transferObjects = Mapper.Map<List<TTransfer>>(domainChunk.Items);

            var transferChunk = Chunk<TTransfer>.From(domainChunk, transferObjects);

            return transferChunk;
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

                return StatusCode((int)HttpStatusCode.Created, transfer);
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
        [Route("{id}")]
        public virtual IActionResult UpdateById(TTransferId id, TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnUpdate(id, value);

                if (transfer == null)
                {
                    return NotFound();
                }

                return Ok(transfer);
            });
        }

        protected virtual TTransfer OnUpdate(TTransferId id, TTransfer value)
        {
            var domain = Mapper.Map<TDomain>(value);
            var domainId = Mapper.Map<TDomainId>(id);

            domain = Service.UpdateById(domainId, domain);

            var transfer = domain == null ? null : Mapper.Map<TTransfer>(domain);

            return transfer;
        }

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TTransfer value)
        {
            return ErrorCheck(() =>
            {
                var transfer = OnUpdate(value);

                if (transfer == null)
                {
                    return NotFound();
                }

                return Ok(transfer);
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

        protected IActionResult WrapCollection<T>(IEnumerable<T> data, string name)
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

            return StatusCode((int)status, wrappedObject);
        }


        [HttpGet]
        [Route("filter-profile")]
        public IActionResult GetFilterProfile()
        {
            // Provide a mechanism for user to decide this (filtering fulltree) 
            var fullTree = true;

            var filteringProfile = _essence.
                FilterInformationService.
                GetFilterProfile<TStorage, TStorageId>(fullTree);

            return Ok(filteringProfile);
        }
    }
}