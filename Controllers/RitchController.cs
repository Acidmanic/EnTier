


using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Controllers{



    public abstract class RitchControllerBase
    <StorageEntity,TransferEntity> : ControllerBase 
    where StorageEntity:class
    {


        protected IObjectMapper Mapper{get; private set;}

        public RitchControllerBase(IObjectMapper mapper)
        {
            Mapper = mapper;
        }
        protected class SafeRunResult{

            public StorageEntity Result{get; set;}
            
            public IActionResult ErrorReturningResult{get; set;}

            public bool Success {get;set;}
            
        }

        protected SafeRunResult SafeRun(Func<StorageEntity> runnable,Func<IActionResult> onError){

            var ret = new SafeRunResult(){Success = true};
            try
            {
                ret.Result = runnable();
            }
            catch (System.Exception)
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
        protected SafeRunResult SafeRun(Func<StorageEntity> runnable){
            return SafeRun(runnable, () => Error());
        }

        protected TransferEntity Map(StorageEntity storage){
            return Mapper.Map<TransferEntity>(storage);
        }

        protected IActionResult Map(SafeRunResult result){
            if(result.Success){

                var transfer = Map(result.Result);

                return Ok(transfer);
            }else{
                return result.ErrorReturningResult;
            }
        }
    }    
}