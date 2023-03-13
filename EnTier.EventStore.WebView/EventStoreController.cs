using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier.EventStore.WebView.ContentProviders;
using EnTier.Reflection;
using EnTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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


        [Route("/{*:path}")]
        public IActionResult Index(string path)
        {
            var contentRoot = Path.Join(this.GetAssemblyDirectory(), "wwwroot");

            var contentProvider = new StaticFileContentProvider(contentRoot, "")
                .AddDefaultDocument("index.html");

            contentProvider.AppendChainAfter(new InMemoryIndexPageContentProvider());


            return new ContentProviderActionResult(contentProvider);
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

            return Ok(events);
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

            return Ok(events);
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
                    TotalEvents = EventStreamRepository.Create(_unitOfWork, p).Count()
                });
            return Ok(streams);
        }

        [HttpGet]
        [Route("stream-by-name/{name}")]
        public IActionResult GetStreamById(string name)
        {
            var profiles = TypeRepository.Instance.Profiles;

            var aggreagate = profiles
                .Where(p => p.EventType.Name.ToLower() == name.ToLower())
                .Select(
                    p => new
                    {
                        StreamName = p.EventType.Name,
                        AggregateType = p.AggregateType.FullName,
                        AggregateRootType = p.AggregateRootType.FullName,
                        EventType = p.EventType.FullName,
                        StreamIdType = p.StreamIdType.FullName,
                        EventIdType = p.EventIdType.FullName,
                        TotalEvents = EventStreamRepository.Create(_unitOfWork, p).Count()
                    }).FirstOrDefault();
            if (aggreagate == null)
            {
                return NotFound();
            }

            return Ok(aggreagate);
        }
    }
}