using EnTier.Components;
using EnTier.Configuration;
using EnTier.DataAccess;
using Microsoft.AspNetCore.Mvc;
using EnTier.Binding;
using EnTier.Repository;
using EnTier.Service;
using System;
using System.Collections.Generic;
using System.Text;
using EnTier.Utility;

namespace EnTier.Controllers
{
    public abstract partial class EnTierControllerBase
        <StorageModel, DomainModel, TransferModel, Tid>
        :ControllerBase, IDisposable, IEnTierController
    where StorageModel : class
    {
        public EnTierControllerBase(IObjectMapper mapper,
                                    IService<DomainModel, Tid> service,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            Initialize(mapper, service, configurationProvider);
        }


        private void Initialize(IObjectMapper mapper,
                                    IService<DomainModel, Tid> service,
                                    IProvider<EnTierConfigurations> configurationProvider)
        {
            //TODO: Manage this requirment
            ReflectionService.Make().Cache(typeof(GenericService<StorageModel, DomainModel, Tid>));
            ReflectionService.Make().Cache(typeof(GenericRepository<StorageModel, Tid>));

            _attributeEagerScopeManager = new AttributeEagerScopeManager<StorageModel>(this);

            ControllerConfigurations = SetupControllerConfigurations();

            Mapper = mapper;

            Service = service;

            EnTierConfigurations = configurationProvider.Create();

            IO = new ControllerIO(EnTierConfigurations, Mapper, this);

        }


        public EnTierControllerBase(IObjectMapper mapper, IService<DomainModel, Tid> service)
        {
            var configurationProvider = new DefaultConfigurationsProvider();

            Initialize(mapper, service, configurationProvider);
        }

        public EnTierControllerBase(IObjectMapper mapper, IProvider<EnTierConfigurations> configurationProvider)
        {
            var service = new ComponentProducer().ProduceService<DomainModel, Tid>();

            Initialize(mapper, service, configurationProvider);
        }


        public EnTierControllerBase(IObjectMapper mapper)
        {
            var configurationProvider = new DefaultConfigurationsProvider();

            var service = new ComponentProducer().ProduceService<DomainModel, Tid>();

            Initialize(mapper, service, configurationProvider);
        }

        public EnTierControllerBase()
        {
            var configurationProvider = new DefaultConfigurationsProvider();

            var service = new ComponentProducer().ProduceService<DomainModel, Tid>();

            var mapper = new ComponentProducer().ProduceMapper();

            Initialize(mapper, service, configurationProvider);
        }
    }
}

