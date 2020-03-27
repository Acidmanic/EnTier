


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Configuration;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Service;
using Utility;
using Repository;
using Components;

namespace Controllers{




    public interface IEnTierController
    {
    }

    public abstract class EnTierControllerBase
    <StorageModel,DomainModel,TransferModel,Tid>
    :ControllerBase,IDisposable,IEnTierController
    where StorageModel:class{

        protected IObjectMapper Mapper {get; private set;}

        internal ControllerConfigurations ControllerConfigurations {get;private set;}

        protected EnTierConfigurations EnTierConfigurations {get;private set;}

        protected ControllerIO IO {get; private set;}

        private AttributeEagerScopeManager<StorageModel> _attributeEagerScopeManager;

        protected IService<DomainModel,Tid> Service{ get; private set;}


        public EnTierControllerBase(IObjectMapper mapper,
                                    IService<DomainModel,Tid> service,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            //TODO: Manage this requirment
            ReflectionService.Make().Cache(typeof(GenericService<StorageModel, DomainModel, Tid>));

            _attributeEagerScopeManager = new AttributeEagerScopeManager<StorageModel>(this);

            ControllerConfigurations = SetupControllerConfigurations();

            Mapper = mapper == null ? new ComponentProducer().ProduceMapper() : mapper;

            Service = service==null?new ComponentProducer().ProduceService<DomainModel,Tid>():service;

            if(configurationProvider==null) configurationProvider = DefaultConfigurationProvider();

            EnTierConfigurations = configurationProvider.Create();

            IO = new ControllerIO(EnTierConfigurations,Mapper,this);

        }

        
        public EnTierControllerBase(IObjectMapper mapper,IService<DomainModel,Tid> service)
        :this(mapper,service,null){        }

        private IProvider<EnTierConfigurations> DefaultConfigurationProvider()
        {
            return new DefaultConfigurationsProvider();
        }

        public EnTierControllerBase(IObjectMapper mapper,IProvider<EnTierConfigurations> configurationProvider)
        :this(mapper,null,configurationProvider){ }

  
        public EnTierControllerBase(IObjectMapper mapper):this(mapper,null,null){}

        public EnTierControllerBase() : this(null, null, null) { }

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


    public abstract class EnTierControllerBase
    <StorageModel, DomainModel, TransferModel>
    : EnTierControllerBase
    <StorageModel, DomainModel, TransferModel, long>
    where StorageModel : class
    {
        public EnTierControllerBase(IObjectMapper mapper) : base(mapper)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IService<DomainModel, long> service) : base(mapper, service)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider) : base(mapper, configurationProvider)
        {
        }

        public EnTierControllerBase(IObjectMapper mapper, IService<DomainModel, long> service, IProvider<EnTierConfigurations> configurationProvider) : base(mapper, service, configurationProvider)
        {
        }
    }
}