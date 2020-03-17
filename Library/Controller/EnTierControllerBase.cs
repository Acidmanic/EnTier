


using System;
using System.Net;
using Configuration;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Utility;

namespace Controllers
{



    [ApiController]
    public abstract partial class EnTierControllerBase
    <DomainEntity,TransferEntity> : ControllerBase 
    {


        protected IObjectMapper Mapper{get; private set;}

        protected EnTierConfigurations EnTierConfigurations {get;set;}

        protected ControllerConfigurations ControllerConfigurations{get;private set;}

        protected ControllerIO IO {get; private set;}

        public EnTierControllerBase(IObjectMapper mapper)
        {
            Mapper = mapper;

            EnTierConfigurations = new DefaultConfigurationsProvider().Create();

            InitiateConfiguration();

            OnControllerInitialize();

            IO = new ControllerIO(EnTierConfigurations,Mapper,this);
        }


        private void InitiateConfiguration()
        {
            ControllerConfigurationBuilder builder = new ControllerConfigurationBuilder();

            Configure(builder);

            ControllerConfigurations = builder.Build();

        }

        protected virtual void Configure(ControllerConfigurationBuilder builder){
            builder.ImplementAll();
        }

        protected virtual void OnControllerInitialize(){}

        
    }    
}