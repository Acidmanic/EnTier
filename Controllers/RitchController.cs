


using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Controllers{



    public abstract class RitchControllerBase
    <StorageEntity,TransferEntity> : ControllerBase 
    where StorageEntity:class
    {
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
    }    
}