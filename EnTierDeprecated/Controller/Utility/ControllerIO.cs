


using System;
using System.Net;
using EnTier.Configuration;
using Microsoft.AspNetCore.Mvc;
using EnTier.Binding;

namespace EnTier.Controllers
{





    public class ControllerIO{


        private EnTierConfigurations _enTierConfigurations;

        private IObjectMapper _mapper;

        private ControllerBase _owner;


        public ControllerIO(EnTierConfigurations enTierConfigurations,
                            IObjectMapper mapper,
                            ControllerBase owner){
            _enTierConfigurations = enTierConfigurations;
            _mapper = mapper;
            _owner = owner;
        }
        public SafeRunResult<TReturn> SafeRun<TReturn>(Func<TReturn> runnable,HttpStatusCode statusCode = HttpStatusCode.InternalServerError){

            var ret = new SafeRunResult<TReturn>(){Success = true};
            try
            {
                ret.Result = runnable();
            }
            catch (Exception ex)
            {

                ret.ErrorReturningResult = _enTierConfigurations.ExposesExceptions
                    ? Error(statusCode,ex)
                    : Error(statusCode);

                ret.Success = false;
                
            }

            return ret;
        }


        public IActionResult Error(HttpStatusCode code,Exception ex){

            return _owner.StatusCode((int) code,
                new {
                    ErrorCode=(int) code,
                    ErrorMessage = ex.Message,
                    ex.StackTrace,
                    ex.Source
                }
            );
        }

        public IActionResult Error(HttpStatusCode error){
            return _owner.StatusCode((int) error,
                new {
                    ErrorCode=(int) error,
                    ErrorMessage = error.ToString()
                }
            );
        }

        public IActionResult Map<TSource,TReturn>(SafeRunResult<TSource> result){
            if(result.Success){

                var transfer = _mapper.Map<TReturn>(result.Result);

                return _owner.Ok(transfer);
            }else{
                return result.ErrorReturningResult;
            }
        }



    }
}