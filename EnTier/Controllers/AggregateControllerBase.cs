using System;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.NamingConventions;
using Acidmanic.Utilities.Results;
using EnTier.EventSourcing;
using EnTier.EventSourcing.Models;
using EnTier.Extensions;
using EnTier.Reflection;
using EnTier.Services;
using EnTier.TransferModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EnTier.Controllers
{
    public abstract class AggregateControllerBase<TAggregateRoot, TEvent, TStreamId, TEventId> : ControllerBase
    {
        protected Type AggregateType { get; }
        protected AggregateIndex AggregateIndex { get; }
        protected IAggregateBuilder AggregateBuilder { get; }

        protected ILogger Logger { get; }

        protected EventSourcedService
            <TAggregateRoot, TEvent, TStreamId, TEventId> Service { get; }


        public AggregateControllerBase(EnTierEssence essence)
        {
            Logger = essence.Logger;

            AggregateBuilder = essence.AggregateBuilder;

            AggregateType = AggregateBuilder.FindAggregateType<TAggregateRoot, TEvent, TStreamId>();

            if (AggregateType == null)
            {
                Logger.LogError("No implemented Aggregate has been found for AggregateRoot: {AggregateRoot} and " +
                                "Event: {Event}, StreamId type of {StreamId} and EventId type of {EventId}",
                    typeof(TAggregateRoot).Name, typeof(TEvent).Name,
                    typeof(TStreamId).Name, typeof(TEventId).Name);
            }

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


        private async Task<Result<IAggregate<TAggregateRoot, TEvent, TStreamId>>> ProvideAggregate
            (TStreamId streamId, bool byStreamId)
        {
            if (byStreamId)
            {
                return await Service.GetAggregateAsync(streamId);
            }

            var newAggregate = AggregateBuilder.Build<TAggregateRoot, TEvent, TStreamId>();

            return new Result<IAggregate<TAggregateRoot, TEvent, TStreamId>>(true, newAggregate);
        }

        private async Task<IActionResult> HandleRequest(HttpContext context, TStreamId streamId, bool byStreamId)
        {
            var foundMethodName = HttpContext.Request.ReadLastPathSegment();

            if (foundMethodName)
            {
                var foundProfile = AggregateIndex.FindProfile(foundMethodName.Value, byStreamId);

                if (foundProfile)
                {
                    if (context.Request.Method.ToLower() != foundProfile.Value.HttpMethod.Method.ToLower())
                    {
                        return StatusCode(405, "This http method is not allowed on this endpoint");
                    }

                    var foundAggregate = await ProvideAggregate(streamId, byStreamId);

                    if (foundAggregate)
                    {
                        var aggregate = foundAggregate.Value;

                        var result = await HttpContext.Request.ExecuteMethod(aggregate,
                            foundProfile.Value.Method,
                            foundProfile.Value.ModelType);

                        if (result.Successful)
                        {
                            await Service.SaveAsync(aggregate);

                            var model = AssembleModel(aggregate.CurrentState, result, foundProfile.Value);

                            return model == null ? (IActionResult)Ok() : Ok(model);
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

        private object AssembleModel(TAggregateRoot aggregateRoot, MethodExecutionResult result, MethodProfile profile)
        {
            if (profile.ReturnsBoth)
            {
                if (result.ReturnsValue)
                {
                    return new
                    {
                        Result = result.ReturnValue,
                        State = aggregateRoot
                    };
                }

                return aggregateRoot;
            }

            if (profile.ReturnsAggregateRootOnly)
            {
                return aggregateRoot;
            }

            if (profile.ReturnsMethodResultOnly && result.ReturnsValue)
            {
                return result.ReturnValue;
            }

            return null;
        }


        [HttpGet]
        [HttpPost]
        [HttpDelete]
        [HttpHead]
        [HttpOptions]
        [HttpPut]
        [HttpPatch]
        [Route("{_?}")]
        public Task<IActionResult> Index()
        {
            return HandleRequest(HttpContext, default, false);
        }
        [HttpGet]
        [HttpPost]
        [HttpDelete]
        [HttpHead]
        [HttpOptions]
        [HttpPut]
        [HttpPatch]
        [Route("{streamId}/{_?}")]
        public Task<IActionResult> Index(TStreamId streamId)
        {
            return HandleRequest(HttpContext, streamId, true);
        }

        [HttpGet]
        [Route("all-items")]
        public virtual IActionResult GetAll()
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