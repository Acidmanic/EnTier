
using System;
using System.Collections.Generic;
using System.Net;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service;
using Utility;
using Plugging;

namespace Controllers{

    public abstract class GenericControllerBase
        <StorageEntity,DomainEntity,TransferEntity,Tid> : 
        EnTierControllerBase<DomainEntity,TransferEntity> 
        ,IDisposable
        where StorageEntity:class
    {

        private IService<DomainEntity,Tid> _service;

        private EagerScopeManager _attributesScope;
        
        protected IService<DomainEntity,Tid> Service{
            get{

                var obj = new Object();

                lock(obj){
                    if (_service == null){
                        _service = ReflectionService.Make()
                            .FindConstructor<IService<DomainEntity,Tid>>
                            (Mapper).Construct();
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
        
        protected GenericControllerBase(
            IObjectMapper mapper,
            IService<DomainEntity,Tid> entityService
            ):base(mapper)
        {

            Service = entityService;
            
        }

        protected GenericControllerBase(
            IObjectMapper mapper):base(mapper)
        {

            Service = null;
            
        }

        protected override void OnControllerInitialize(){
            base.OnControllerInitialize();

            _attributesScope = new EagerAttributeProcessor()
                .MarkEagers<StorageEntity>(this);
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll(){

            if(!ControllerConfigurations.ImplementsGetAll) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(() => Service.GetAll(), HttpStatusCode.NoContent);

            return Map<List<DomainEntity>,List<TransferEntity>>(result);

        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(Tid id){

            if(!ControllerConfigurations.ImplementsGetById) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(()=> Service.GetById(id), HttpStatusCode.NotFound);

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TransferEntity entity){

            if(!ControllerConfigurations.ImplementsCreateNew) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);

            var result = SafeRun(()=>Service.CreateNew(domain));

            return Map<DomainEntity,TransferEntity>(result);
        }
        

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TransferEntity entity){

            if(!ControllerConfigurations.ImplementsUpdate) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);

            var result = SafeRun(()=> Service.Update(domain));

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteById(Tid id){

            if(!ControllerConfigurations.ImplementsDeleteById) return Error(HttpStatusCode.MethodNotAllowed);

            var result = SafeRun(()=> Service.DeleteById(id));

            return Map<DomainEntity,TransferEntity>(result);

        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(TransferEntity entity){

            if(!ControllerConfigurations.ImplementsDeleteByEntity) return Error(HttpStatusCode.MethodNotAllowed);

            var domain = Mapper.Map<DomainEntity>(entity);
            
            var result = SafeRun(()=> Service.DeleteByEntity(domain));

            return Map<DomainEntity,TransferEntity>(result);
        }

        public virtual void Dispose()
        {
            if (_attributesScope != null){
                _attributesScope.Dispose();
            }
        }
    }


    public abstract class GenericControllerBase<StorageEntity, DomainEntity, TransferEntity>
        : GenericControllerBase<StorageEntity, DomainEntity, TransferEntity, long>
        where StorageEntity : class
    {
        protected GenericControllerBase(IObjectMapper mapper, IService<DomainEntity, long> entityService) : base(mapper, entityService)
        {
        }

        protected GenericControllerBase(IObjectMapper mapper) : base(mapper)
        {
        }
    }

}