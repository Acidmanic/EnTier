using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using EnTier.Configuration;
using EnTier.DataAccess;
using Microsoft.AspNetCore.Mvc;
using EnTier.Binding;
using EnTier.Service;
using EnTier.Utility;
using EnTier.Repository;
using EnTier.Components;

namespace EnTier.Controllers
{




    public interface IEnTierController
    {
    }

    public abstract partial class EnTierControllerBase
    <StorageModel,DomainModel,TransferModel,Tid>
    :ControllerBase,IDisposable,IEnTierController
    where StorageModel:class{

        protected IObjectMapper Mapper {get; private set;}

        internal ControllerConfigurations ControllerConfigurations {get;private set;}

        protected EnTierConfigurations EnTierConfigurations {get;private set;}

        protected ControllerIO IO {get; private set;}

        private AttributeEagerScopeManager<StorageModel> _attributeEagerScopeManager;

        protected IService<DomainModel,Tid> Service{ get; private set;}

        private ControllerConfigurations SetupControllerConfigurations()
        {
            var builder = new ControllerConfigurationBuilder();

            OnConfiguringController(builder);

            return builder.Build();
        }

        protected virtual void OnConfiguringController(ControllerConfigurationBuilder builder){
            builder.ImplementAll();
        }


        [HttpGet]
        [Route("")]
        public virtual IActionResult GetAll(){
            if(!ControllerConfigurations.ImplementsGetAll) return IO.Error(HttpStatusCode.MethodNotAllowed);
            
            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var result = IO.SafeRun(() => Service.GetAll());

                return IO.Map<List<DomainModel>,List<TransferModel>>(result);
            }

        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(Tid id){

            if(!ControllerConfigurations.ImplementsGetById) return IO.Error(HttpStatusCode.MethodNotAllowed);

            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var result = IO.SafeRun(()=> Service.GetById(id), HttpStatusCode.NotFound);

                return IO.Map<DomainModel,TransferModel>(result);
            }
        }

        [HttpPost]
        [Route("")]
        public virtual IActionResult CreateNew(TransferModel entity){

            if(!ControllerConfigurations.ImplementsCreateNew) return IO.Error(HttpStatusCode.MethodNotAllowed);

            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var domain = Mapper.Map<DomainModel>(entity);

                var result = IO.SafeRun(()=>Service.CreateNew(domain));

                return IO.Map<DomainModel,TransferModel>(result);
            }
        }
        

        [HttpPut]
        [Route("")]
        public virtual IActionResult Update(TransferModel entity){

            if(!ControllerConfigurations.ImplementsUpdate) return IO.Error(HttpStatusCode.MethodNotAllowed);

            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var domain = Mapper.Map<DomainModel>(entity);

                var result = IO.SafeRun(()=> Service.Update(domain));

                return IO.Map<DomainModel,TransferModel>(result);
            }

        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteById(Tid id){

            if(!ControllerConfigurations.ImplementsDeleteById) return IO.Error(HttpStatusCode.MethodNotAllowed);

            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var result = IO.SafeRun(()=> Service.DeleteById(id));

                return IO.Map<DomainModel,TransferModel>(result);
            }

        }

        [HttpDelete]
        [Route("")]
        public IActionResult Delete(TransferModel entity){

            if(!ControllerConfigurations.ImplementsDeleteByEntity) return IO.Error(HttpStatusCode.MethodNotAllowed);

            using(var s = new MethodAttributesEagerScope<StorageModel>(MethodBase.GetCurrentMethod()))
            {
                var domain = Mapper.Map<DomainModel>(entity);
                
                var result = IO.SafeRun(()=> Service.DeleteByEntity(domain));

                return IO.Map<DomainModel,TransferModel>(result);
            }
        }

        public void Dispose()
        {
            _attributeEagerScopeManager.Dispose();
        }

    }

}