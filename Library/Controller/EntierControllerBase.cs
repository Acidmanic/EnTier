


using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Configuration;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Service;
using Utility;

namespace Controllers{




    public class EnTierControllerBase
    <StorageModel,DomainModel,TransferModel,Tid>
    :ControllerBase,IDisposable
    where StorageModel:class{

        protected IObjectMapper Mapper {get; private set;}

        protected ControllerConfigurations ControllerConfigurations {get;private set;}

        protected EnTierConfigurations EnTierConfigurations {get;private set;}

        protected ControllerIO IO {get; private set;}

        private AttributeEagerScopeManager<StorageModel> _attributeEagerScopeManager;

        protected IService<DomainModel,Tid> Service{ get; private set;}


        private void InitializeDependencies(IObjectMapper mapper,
                                    IService<DomainModel,Tid> service,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            
            _attributeEagerScopeManager = new AttributeEagerScopeManager<StorageModel>(this);
            
            Mapper = mapper;

            Service = service;

            EnTierConfigurations = configurationProvider.Create();

            ControllerConfigurations = SetupControllerCOnfigurations();

            IO = new ControllerIO(EnTierConfigurations,Mapper,this);

        }

        public EnTierControllerBase(IObjectMapper mapper,
                                    IService<DomainModel,Tid> service,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            
            InitializeDependencies(mapper,service,configurationProvider);

        }

        public EnTierControllerBase(IObjectMapper mapper,
                                    IService<DomainModel,Tid> service)
        {
            
            InitializeDependencies(mapper,service,DefaultConfigurationProvider());

        }

        private IProvider<EnTierConfigurations> DefaultConfigurationProvider()
        {
            return new DefaultConfigurationsProvider();
        }

        public EnTierControllerBase(IObjectMapper mapper,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            InitializeDependencies(mapper,DefaultService(mapper),configurationProvider);

        }

        private IService<DomainModel, Tid> DefaultService(IObjectMapper mapper)
        {
            return ReflectionService.Make()
                .FindConstructorByPriority<
                                IService<DomainModel,Tid>,
                                GenericService<StorageModel,DomainModel,Tid>>
                                (mapper).Construct();
        }

        public EnTierControllerBase(IObjectMapper mapper)
        {
            InitializeDependencies(mapper,
                DefaultService(mapper),
                DefaultConfigurationProvider());

        }

        private ControllerConfigurations SetupControllerCOnfigurations()
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


    public class EnTierControllerBase
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