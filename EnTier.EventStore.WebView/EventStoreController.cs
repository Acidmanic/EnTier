using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EnTier.Reflection;
using EnTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EnTier.EventStore.WebView
{
    [ApiController]
    [Route("event-store")]
    public class EventStoreController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventStoreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        private class OkEventsResult : IActionResult
        {
            private readonly IEnumerable<EventWrap> _events;

            public OkEventsResult(IEnumerable<EventWrap> events)
            {
                _events = events;
            }

            public async Task ExecuteResultAsync(ActionContext context)
            {
                var wrapperObject = new { Events = _events };

                var response = context.HttpContext.Response;
                
                var json = JsonConvert.SerializeObject(wrapperObject);

                var jsonData = Encoding.Default.GetBytes(json);

                await response.Body.WriteAsync(jsonData);

            }
        } 
        
       
        [HttpGet]
        [Route("stream/{streamName}")]
        public IActionResult GetAllStreams(
            string streamName,
            [FromQuery] int from = 0,
            [FromQuery] int count = int.MaxValue)
        {
            var profile = TypeRepository.Instance.ProfileByStreamName(streamName);

            if (!profile)
            {
                return NotFound();
            }

            var repository = EventStreamRepository.Create(_unitOfWork, profile);

            var events = repository.ReadAll(from, count);

            return new OkEventsResult(events);
        }

        [HttpGet]
        [Route("stream/{streamName}/{streamId}")]
        public IActionResult GetStreamById(string streamName, string streamId,
            [FromQuery] int from = 0,
            [FromQuery] int count = int.MaxValue)
        {
            var profile = TypeRepository.Instance.ProfileByStreamName(streamName);

            if (!profile)
            {
                return NotFound();
            }

            var streamIdValue = streamId.ParseOrDefault(profile.Value.StreamIdType);

            if (streamIdValue == null)
            {
                return BadRequest();
            }

            var repository = EventStreamRepository.Create(_unitOfWork, profile);

            var events = repository.ReadAll(streamIdValue, from, count);

            return new OkEventsResult(events);
        }


        [HttpGet]
        [Route("streams")]
        public IActionResult GetStreams()
        {
            var profiles = TypeRepository.Instance.Profiles;

            var streams = profiles.Select(
                p => new
                {
                    StreamName = p.EventType.Name,
                    AggregateType = p.AggregateType.FullName,
                    AggregateRootType = p.AggregateRootType.FullName,
                    EventType = p.EventType.FullName,
                    StreamIdType = p.StreamIdType.FullName,
                    EventIdType = p.EventIdType.FullName,
                    TotalEvents = EventStreamRepository.Create(_unitOfWork,p).Count()
                });
            return Ok(streams);
        }


        // [HttpGet]
        // public IActionResult GetAll(string streamName)
        // {
        //     var profile = TypeRepository.Instance.ProfileByStreamName(streamName);
        //
        //     if (!profile)
        //     {
        //         return NotFound();
        //     }
        //
        //     var repository = EventStreamRepository.Create(_unitOfWork, profile);
        //
        //     var allEvents = repository.ReadStream();
        //
        //     return Ok(allEvents);
        // }
    }
}