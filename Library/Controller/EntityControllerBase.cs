
using System;
using System.Collections.Generic;
using System.Net;
using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Utility;

namespace Controllers{

    public abstract class EntityControllerBase
        <StorageEntity,DomainEntity,TransferEntity,Tid> : 
        RitchControllerBase<DomainEntity,TransferEntity> 
        where StorageEntity:class
    {


        private IService<DomainEntity,Tid> _service;
        protected IService<DomainEntity,Tid> EntityService{
            get{

                var obj = new Object();

                lock(obj){
                    if (_service == null){
                        _service = ReflectionService.Make()
                            .FindConstructor<IService<DomainEntity,Tid>>
                            (Mapper)();
                    }
                    if( _service == null ){
                        _service = new GenericService
                            <StorageEntity,DomainEntity,Tid>(Mapper);
                    }
                }

                return _service;
            }
            set{
                _service = value;
            }
        }

        private ControllerConfigurations _configurations;

        protected EntityControllerBase(
            IObjectMapper mapper,
            IService<DomainEntity,Tid> entityService
            ):base(mapper)
        {

            EntityService = entityService;

            InitiateConfiguration();
            
        }

        protected EntityControllerBase(
            IObjectMapper mapper):base(mapper)
        {

            EntityService = null;

            InitiateConfiguration();
            
        }

        private void InitiateConfiguration()
        {
            ControllerConfigurationBuilder builder = new ControllerConfigurationBuilder();

            Configure(builder);

            _configurations = builder.Build();
        }

        protected virtual void Configure(ControllerConfigurationBuilder builder){
            builder.ImplementAll();
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll(){

            if(!_configurations.ImplementsGetAll) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(() => EntityService.GetAll(), ()=> NotFound());

            return Map<List<DomainEntity>,List<TransferEntity>>(result);

        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(Tid id){

            if(!_configurations.ImplementsGetById) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(()=> EntityService.GetById(id),()=> NotFound());

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TransferEntity entity){

            if(!_configurations.ImplementsCreateNew) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);

            var result = SafeRun(()=>EntityService.CreateNew(domain),()=>Error());

            return Map<DomainEntity,TransferEntity>(result);
        }
        

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TransferEntity entity){

            if(!_configurations.ImplementsUpdate) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);

            var result = SafeRun(()=> EntityService.Update(domain));

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteById(Tid id){

            if(!_configurations.ImplementsDeleteById) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(()=> EntityService.DeleteById(id),() => NotFound());

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(TransferEntity entity){

            if(!_configurations.ImplementsDeleteByEntity) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);
            
            var result = SafeRun(()=> EntityService.DeleteByEntity(domain),() => NotFound());

            return Map<DomainEntity,TransferEntity>(result);
        }
    }


    public abstract class EntityControllerBase<StorageEntity, DomainEntity, TransferEntity>
        : EntityControllerBase<StorageEntity, DomainEntity, TransferEntity, long>
        where StorageEntity : class
    {
        protected EntityControllerBase(IObjectMapper mapper, IService<DomainEntity, long> entityService) : base(mapper, entityService)
        {
        }

        protected EntityControllerBase(IObjectMapper mapper) : base(mapper)
        {
        }
    }

}