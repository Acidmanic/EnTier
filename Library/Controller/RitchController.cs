


using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Controllers{



    [ApiController]
    public abstract class RitchControllerBase
    <DomainEntity,TransferEntity> : ControllerBase 
    {


        protected IObjectMapper Mapper{get; private set;}

        public RitchControllerBase(IObjectMapper mapper)
        {
            Mapper = mapper;
        }
        protected class SafeRunResult<TReturn>{

            public TReturn Result{get; set;}
            
            public IActionResult ErrorReturningResult{get; set;}

            public bool Success {get;set;}
            
        }

        protected SafeRunResult<TReturn> SafeRun<TReturn>(Func<TReturn> runnable,Func<IActionResult> onError){

            var ret = new SafeRunResult<TReturn>(){Success = true};
            try
            {
                ret.Result = runnable();
            }
            catch (Exception)
            {
                ret.Success = false;
                ret.ErrorReturningResult = onError();
            }

            return ret;
        }

        protected IActionResult Error(){
            return Error(HttpStatusCode.InternalServerError);
        }


        protected IActionResult Error(HttpStatusCode error ){
            return StatusCode((int) error,
                new {
                    ErrorCode=(int) error,
                    ErrorMessage = error.ToString()
                }
            );
        }
        protected SafeRunResult<TReturn> SafeRun<TReturn>(Func<TReturn> runnable){
            return SafeRun(runnable, () => Error());
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