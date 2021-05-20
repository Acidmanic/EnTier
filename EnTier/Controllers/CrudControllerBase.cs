using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using EnTier.Mapper;
using EnTier.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Controllers
{
    public abstract class CrudControllerBase<TTransfer, TDomain, TId> : ControllerBase
        where TTransfer : class, new()
        where TDomain : class, new()
    {
        public IMapper Mapper { get; private set; }

        public ICrudService<TDomain, TId> Service { get; private set; }


        private IActionResult ErrorCheck(Func<IActionResult> code)
        {
            try
            {
                return code.Invoke();
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, e);
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
        public virtual IActionResult GetById(TId id)
        {
            return ErrorCheck(() =>
            {
                var domain = Service.GetById(id);

                if (domain == null)
                {
                    return NotFound();
                }

                var transfer = Mapper.Map<List<TTransfer>>(domain);

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
        public IActionResult DeleteById(TId id)
        {
            return ErrorCheck(() =>
            {
                var success = Service.RemoveById(id);

                if (!success)
                {
                    return NotFound();
                }
                return Ok();
            });
        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(TTransfer value)
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