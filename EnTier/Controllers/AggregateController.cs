using System;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.NamingConventions;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing;
using EnTier.Extensions;
using EnTier.Services;
using EnTier.TransferModels;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Controllers
{
    public abstract class AggregateController<TAggregateRoot, TEvent, TStreamId, TEventId> : ControllerBase
    {
        protected Type AggregateType { get; }
        protected  AggregateIndex AggregateIndex { get; }
        protected  AggregateBuilder AggregateBuilder { get; }

        protected  EventSourcedService
            <TAggregateRoot, TEvent, TStreamId, TEventId> Service { get; }
        
        

        public AggregateController(EnTierEssence essence)
        {
            AggregateBuilder = new AggregateBuilder(t => null);
            AggregateType = AggregateBuilder.FindAggregateType<TAggregateRoot, TEvent, TStreamId>();
            AggregateIndex = new AggregateIndex(AggregateType)
            {
                ModerateMethodNames = ModerateUriNames
            };
            Service = new EventSourcedService<TAggregateRoot, TEvent, TStreamId, TEventId>(essence);
        }


        [HttpGet]
        [Route("available-methods")]
        public IActionResult GetAvailableMethods()
        {
            var methods = AggregateIndex.MethodProfiles.Values
                .Select(p => new MethodDeclaration().Load(p)).ToList();

            return Ok(new
            {
                methods = methods
            });
        }

        [HttpPost]
        [Route("{streamId}/{_?}")]
        public async Task<IActionResult> Index(TStreamId streamId)
        {
            var foundMethodName = HttpContext.Request.ReadLastPathSegment();

            if (foundMethodName)
            {
                var foundAggregate = await Service.GetAggregateAsync(streamId);


                if (foundAggregate)
                {
                    var aggregate = foundAggregate.Value;
                    
                    var foundProfile = AggregateIndex.FindProfile(foundMethodName.Value);
                    
                    if (foundProfile)
                    {
                        var result = await HttpContext.Request.ExecuteMethod(aggregate,
                            foundProfile.Value.Method,
                            foundProfile.Value.ModelType);

                        if (result.Successful)
                        {
                            await Service.SaveAsync(aggregate);

                            return Ok(aggregate.CurrentState);
                        }

                        if (ReflectExceptions)
                        {
                            return BadRequest(new
                            {
                                exceptionMessage = result.Exception.Message,
                                exceptionName = result.Exception.GetType().Name,
                            });
                        }

                        return BadRequest();
                    }    
                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("all-items")]
        public IActionResult GetAll()
        {
            var all = Service.GetAll()
                .Select(a => a.CurrentState);


            return Ok(new
            {
                items = all
            });
        }


        protected virtual bool ReflectExceptions => false;

        protected Result<ConventionDescriptor> ModerateUriNames =>
            new Result<ConventionDescriptor>(true, ConventionDescriptor.Standard.Kebab);
    }
}