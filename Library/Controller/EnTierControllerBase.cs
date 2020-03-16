


using System;
using System.Net;
using Configuration;
using Microsoft.AspNetCore.Mvc;
using Plugging;
using Utility;

namespace Controllers{



    [ApiController]
    public abstract class EnTierControllerBase
    <DomainEntity,TransferEntity> : ControllerBase 
    {


        protected IObjectMapper Mapper{get; private set;}

        protected EnTierConfigurations EnTierConfigurations {get;set;}

        protected ControllerConfigurations ControllerConfigurations{get;private set;}

        public EnTierControllerBase(IObjectMapper mapper)
        {
            Mapper = mapper;

            EnTierConfigurations = new DefaulConfigurationsProvider().Create();

            InitiateConfiguration();

            OnControllerInitialize();
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

        protected class SafeRunResult<TReturn>{

            public TReturn Result{get; set;}
            
            public IActionResult ErrorReturningResult{get; set;}

            public bool Success {get;set;}
            
        }

        protected SafeRunResult<TReturn> SafeRun<TReturn>(Func<TReturn> runnable,HttpStatusCode statusCode = HttpStatusCode.InternalServerError){

            var ret = new SafeRunResult<TReturn>(){Success = true};
            try
            {
                ret.Result = runnable();
            }
            catch (Exception ex)
            {

                ret.ErrorReturningResult = EnTierConfigurations.ExposesExceptions
                    ? Error(statusCode,ex)
                    : Error(statusCode);

                ret.Success = false;
                
            }

            return ret;
        }

        protected IActionResult Error(HttpStatusCode code,Exception ex){

            return StatusCode((int) code,
                new {
                    ErrorCode=(int) code,
                    ErrorMessage = ex.Message,
                    ex.StackTrace,
                    ex.Source
                }
            );
        }


        protected IActionResult Error(HttpStatusCode error){
            return StatusCode((int) error,
                new {
                    ErrorCode=(int) error,
                    ErrorMessage = error.ToString()
                }
            );
        }

        protected IActionResult Map<TSource,TReturn>(SafeRunResult<TSource> result){
            if(result.Success){

                var transfer = Mapper.Map<TReturn>(result.Result);

                return Ok(transfer);
            }else{
                return result.ErrorReturningResult;
            }
        }
    }    
}